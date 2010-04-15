using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using SPGen2010.Components.Windows;
using SPGen2010.Components.Generators.Extensions.Generic;
using SPGen2010.Components.Generators.Extensions.MsSql;
using SPGen2010.Components.Generators.Extensions.MySmo;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;

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

            var sb = new StringBuilder();

            #endregion

            #region Gen

            var pks = t.GetPrimaryKeyColumns();             // 主键集
            var wcs = t.GetWriteableColumns();              // 可填字段集
            var mwcs = t.GetMustWriteColumns();             // 必填字段集

            var tn = t.Name.Escape();                       // 表名
            var ts = t.Schema.Escape();                     // 表架构名
            var spn = "[" + ts + @"].[" + tn + @"_Insert]"; // 存储过程名

            // 头生成
            sb.Append(@"
-- 表    ：[" + ts + @"].[" + tn + @"]
-- 功能  ：添加一行数据
-- 返回值：受影响行数（成功）; 负数（失败）
-- -1: 某些必填字段为空
-- -2: 主键冲突
-- -3: 外键无效
-- -4: 添加失败
CREATE PROCEDURE " + spn + @" (");

            // 参数生成
            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var cn = c.Name.Escape();
                /*
       @xxx                             nvarchar(max)                      = NULL
     , @xxxx                            .......                            .....
                 */
                sb.Append(@"
    " + (i > 0 ? ", " : "  ") + ("@" + cn).FillSpace(40) + c.GetParmDeclareStr().FillSpace(40) + "= NULL");
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
                var cn = c.Name.Escape();
                if (mwcs.Contains(c))
                {
                    sb.Append(@"
    IF @" + cn + @" IS NULL" + (c.DataType.CheckIsStringType() ? ("-- OR LEN(@" + cn + @") = 0") : ("")) + @"
        RETURN -1;
");
                }
                else
                {
                    sb.Append(@"
    IF @" + cn + @" IS NULL SET @" + cn + @" = " + c.DefaultConstraint.Text + @";
");
                }
            }

            //判断主键重复
            //判断是否存在自增主键
            bool hasIdentityCol = false;
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
                    string cn = c.Name.Escape();
                    if (i > 0) sb.Append(@" AND ");
                    sb.Append(@"[" + c.Name.Escape() + @"] = @" + cn);
                }
                sb.Append(@"
    ) RETURN -2;
");
            }

            //判断外键字段是否在外键表中存在
            foreach (var fk in t.ForeignKeys)
            {
                var ft = t.ParentDatabase.Tables.Find(fk.ReferencedTable, fk.ReferencedTableSchema);
                sb.Append(@"
    IF NOT EXISTS (
        SELECT 1 FROM [" + ts + @"].[" + ft.Name.Escape() + @"]
         WHERE ");
                for (int i = 0; i < fk.Columns.Count; i++)
                {
                    var fkc = fk.Columns[i];
                    var c = t.Columns.Find(fkc.Name);
                    var cn = c.Name.Escape();

                    if (i > 0) sb.Append(@" AND ");
                    sb.Append("(" + (c.Nullable ? (" @" + cn + @" IS NULL OR ") : "") + @"[" + fkc.ReferencedColumn.Escape() + @"] = @" + cn + @")");
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
                var cn = c.Name.Escape();
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
                var cn = c.Name.Escape();
                sb.Append(@"
        " + (i > 0 ? ", " : "  ") + "@" + cn);
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
