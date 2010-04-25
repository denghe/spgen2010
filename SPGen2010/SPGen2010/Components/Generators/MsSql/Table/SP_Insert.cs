using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;

using SPGen2010.Components.Windows;
using SPGen2010.Components.Generators.Extensions.Generic;
using SPGen2010.Components.Generators.Extensions.MsSql;
using SPGen2010.Components.Modules.MySmo;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;

namespace SPGen2010.Components.Generators.MsSql.Table
{
    class SP_Insert : IGenerator
    {
        #region Settings

        public SqlElementTypes TargetSqlElementType
        {
            get { return SqlElementTypes.Table; }
        }
        public Dictionary<GenProperties, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    this._properties = new Dictionary<GenProperties, object>();
                    this._properties.Add(GenProperties.Name, "SP/Insert/1");
                    this._properties.Add(GenProperties.Caption, "SP：插入一行（豪华版）");
                    this._properties.Add(GenProperties.Group, "SP");
                    this._properties.Add(GenProperties.Tips, "");
                }
                return this._properties;
            }
        }
        private Dictionary<GenProperties, object> _properties = null;

        #endregion

        #region Validate

        /// <summary>
        /// condations:
        /// </summary>
        public bool Validate(params Oe.NodeBase[] targetElements)
        {
            var oe_t = (Oe.Table)targetElements[0];
            var t = WMain.Instance.MySmoProvider.GetTable(oe_t);
            var wcs = t.GetWriteableColumns();
            return wcs.Count > 0;
        }

        #endregion

        public GenResult Generate(params Oe.NodeBase[] targetElements)
        {
            #region Init

            var gr = new GenResult(GenResultTypes.CodeSegment);
            var oe_t = (Oe.Table)targetElements[0];
            var t = WMain.Instance.MySmoProvider.GetTable(oe_t);
            var t_fts = from ForeignKey fk in t.ForeignKeys
                      select WMain.Instance.MySmoProvider.GetTable(
                          new Oe.Table { Parent = oe_t.Parent, Name = fk.ReferencedTable, Schema = fk.ReferencedTableSchema }
                      );

            var sb = new StringBuilder();

            #endregion

            #region Gen

            var pks = t.GetPrimaryKeyColumns();             // 主键集
            var wcs = t.GetWriteableColumns();              // 可填字段集
            var mwcs = t.GetMustWriteColumns();             // 必填字段集

            var tn = t.Name.EscapeToSqlName();                       // 表名
            var ts = t.Schema.EscapeToSqlName();                     // 表架构名
            var spn = "[" + ts + @"].[" + tn + @"_Insert]"; // 存储过程名

            // 头生成
            sb.Append(@"
-- 表    ：[" + ts + @"].[" + tn + @"]
-- 功能  ：添加一行数据
-- 返回值：INT （成功：受影响行数; 失败：负数）
-- -1: 某些必填字段为空
-- -2: 主键冲突
-- -3: 外键无效
-- -4: 添加失败
CREATE PROCEDURE " + spn + @" (");

            // 参数生成
            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var pn = c.Name.EscapeToParmName();
                /*
       @xxx                             nvarchar(max)                      = NULL
     , @xxxx                            .......                            .....
                 */
                sb.Append(@"
    " + (i > 0 ? ", " : "  ") + ("@" + pn).FillSpace(30) + c.GetParmDeclareStr().FillSpace(30) + "= NULL");
            }

            // 身体生成
            sb.Append(@"
) AS
BEGIN
    SET NOCOUNT ON;
");
            // 前置判断生成
            //判断必填字段是否填写了空值
            foreach (var c in wcs)
            {
                if (c.Nullable) continue;
                var pn = c.Name.EscapeToParmName();
                if (mwcs.Contains(c))
                {
                    sb.Append(@"
    IF @" + pn + @" IS NULL" + (c.DataType.CheckIsStringType() ? ("-- OR LEN(@" + pn + @") = 0") : ("")) + @"
        RETURN -1;
");
                }
                else
                {
                    sb.Append(@"
    IF @" + pn + @" IS NULL SET @" + pn + @" = " + c.DefaultConstraint.Text + @";
");
                }
            }

            //判断主键重复
            //判断是否存在自增主键
            var hasIdentityCol = false;
            foreach (var c in pks)
            {
                if (c.Identity)
                {
                    hasIdentityCol = true;
                    break;
                }
            }
            if (!hasIdentityCol)
            {
                sb.Append(@"
    IF EXISTS (
       SELECT 1 FROM [" + ts + @"].[" + tn + @"]
--         WITH (TABLOCK, HOLDLOCK)
        WHERE ");
                for (int i = 0; i < pks.Count; i++)
                {
                    var c = pks[i];
                    var cn = c.Name.EscapeToSqlName();
                    var pn = c.Name.EscapeToParmName();
                    if (i > 0) sb.Append(@" AND ");
                    sb.Append(@"[" + cn + @"] = @" + pn);
                }
                sb.Append(@"
    ) RETURN -2;
");
            }

            //判断外键字段是否在外键表中存在
            foreach (var fk in t.ForeignKeys)
            {
                var ft = t_fts.Find(fk.ReferencedTable, fk.ReferencedTableSchema);
                var fts = ft.Schema.EscapeToSqlName();
                var ftn = ft.Name.EscapeToSqlName();
                sb.Append(@"
    IF NOT EXISTS (
        SELECT 1 FROM [" + fts + @"].[" + ftn + @"]
         WHERE ");
                for (int i = 0; i < fk.Columns.Count; i++)
                {
                    var fkc = fk.Columns[i];
                    var fkcrn = fkc.ReferencedColumn.EscapeToSqlName();

                    var c = t.Columns.Find(fkc.Name);
                    var cn = c.Name.EscapeToSqlName();
                    var pn = c.Name.EscapeToSqlName();

                    if (i > 0) sb.Append(@" AND ");
                    sb.Append("(" + (c.Nullable ? (" @" + cn + @" IS NULL OR ") : "") + @"[" + fkcrn + @"] = @" + pn + @")");
                }
                sb.Append(@"
    ) RETURN -3;
");
            }

            // 具体插入操作生成
            sb.Append(@"

/*
    --prepare trans & error
    DECLARE @TranStarted bit, @ReturnValue int;
    SELECT @TranStarted = 0, @ReturnValue = 0;
    IF @@TRANCOUNT = 0 
    BEGIN
        BEGIN TRANSACTION;
        SET @TranStarted = 1
    END;
*/

    DECLARE @ERROR INT, @ROWCOUNT INT;

    INSERT INTO [" + ts + @"].[" + tn + @"] (");
            var opts = "";
            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var cn = c.Name.EscapeToSqlName();
                sb.Append(@"
        " + (i > 0 ? ", " : "  ") + "[" + cn + @"]");
                opts += (i > 0 ? ", " : "") + "Inserted.[" + cn + @"]";
            }
            sb.Append(@"
    )
--    OUTPUT " + opts + @"
    VALUES (");
            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var pn = c.Name.EscapeToParmName();
                sb.Append(@"
        " + (i > 0 ? ", " : "  ") + "@" + pn);
            }
            sb.Append(@"
    );");
            sb.Append(@"

    SELECT @ERROR = @@ERROR, @ROWCOUNT = @@ROWCOUNT;
    IF @ERROR <> 0 OR @ROWCOUNT = 0
    BEGIN
/*
        @ReturnValue = -4;
        GOTO Cleanup;
*/
        RETURN -4;
    END

/*
    @ReturnValue = @ROWCOUNT;
    GOTO Cleanup;
*/

    RETURN @ROWCOUNT;

/*
    --cleanup trans
    IF @TranStarted = 1 COMMIT TRANSACTION;
    RETURN @ReturnValue;
Cleanup:
    IF @TranStarted = 1 ROLLBACK TRANSACTION;
    RETURN @ReturnValue;
*/

END
");

            #endregion

            #region return

            gr.CodeSegment.first = this.Properties[GenProperties.Caption].ToString();
            gr.CodeSegment.second = sb.ToString();

            return gr;

            #endregion
        }

    }
}
