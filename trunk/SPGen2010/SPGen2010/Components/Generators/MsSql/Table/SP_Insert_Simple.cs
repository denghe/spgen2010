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
    class SP_Insert_Simple : IGenerator
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
                    this._properties.Add(GenProperties.Name, "SP/Insert/2");
                    this._properties.Add(GenProperties.Caption, "SP：插入一行（精简版）");
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
-- 表： 表 " + t.ToString() + @"
-- 功能：添加一行数据
-- 返回值：受影响行数（成功）; 负数（失败）
-- -1: 添加失败
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

            // 具体插入操作生成
            sb.Append(@"

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
