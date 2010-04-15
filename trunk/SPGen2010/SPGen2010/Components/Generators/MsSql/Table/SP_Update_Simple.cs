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
    class SP_Update_Simple : IGenerator
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
                    this._properties.Add(GenProperties.Name, "SP/Update/2");
                    this._properties.Add(GenProperties.Caption, "SP：更新一行（精简版1）");
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
            var pks = t.GetPrimaryKeyColumns();
            return wcs.Count > 0 && pks.Count > 0;
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

            var tn = t.Name.EscapeToSqlName();                       // 表名
            var ts = t.Schema.EscapeToSqlName();                     // 表架构名
            var spn = "[" + ts + @"].[" + tn + @"_Insert]"; // 存储过程名

            // 头生成
            sb.Append(@"
-- 表    ：[" + ts + @"].[" + tn + @"]
-- 功能  ：根据*主键*更新一行数据
-- 返回值：INT （成功：受影响行数; 失败：负数）
-- -1：更新失败
CREATE PROCEDURE " + spn + @" (");

            // 参数生成
            /*
       @Original_xxx                int
     , @Original_...                ......
             */
            for (int i = 0; i < pks.Count; i++)
            {
                var c = pks[i];
                var pn = c.Name.EscapeToParmName();
                sb.Append(@"
    " + (i > 0 ? ", " : "  ") + ("@Original_" + pn).FillSpace(30) + c.GetParmDeclareStr());
            }
            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var pn = c.Name.EscapeToParmName();
                /*
       @xxx                         nvarchar(max)
     , @xxxx                        .......
                 */
                sb.Append(@"
    " + (i > 0 ? ", " : "  ") + ("@" + pn).FillSpace(30) + c.GetParmDeclareStr());
            }

            // 身体生成
            sb.Append(@"
) AS
BEGIN
    SET NOCOUNT ON;
");

            // 具体插入操作生成
            sb.Append(@"

    DECLARE @ERROR INT, @ROWCOUNT INT;

    UPDATE [" + ts + @"].[" + tn + @"]
       SET ");

            for (int i = 0; i < wcs.Count; i++)
            {
                var c = wcs[i];
                var cn = c.Name.EscapeToSqlName();
                var pn = c.Name.EscapeToParmName();
                sb.Append((i > 0 ? @"
         , " : "") + ("[" + cn + @"]").FillSpace(30) + "= @" + pn);
            }
            var s = "";
            for (int i = 0; i < pks.Count; i++)
            {
                var c = wcs[i];
                var cn = c.Name.EscapeToSqlName();
                var pn = c.Name.EscapeToParmName();
                if (i > 0) s += " AND ";
                s += @"[" + cn + @"] = @Original_" + pn;
            }
            sb.Append(@"
--    OUTPUT ");
            for (int i = 0; i < t.Columns.Count; i++)
            {
                var c = t.Columns[i];
                var cn = c.Name.EscapeToSqlName();
                if (i > 0) sb.Append(", ");
                sb.Append("Inserted.[" + cn + @"]");
            }
            if (s.Length > 0) sb.Append(@"
     WHERE " + s);
            sb.Append(@";

    SELECT @ERROR = @@ERROR, @ROWCOUNT = @@ROWCOUNT;
    IF @ERROR <> 0
    BEGIN
        RETURN -1;
    END

    RETURN @ROWCOUNT;

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
