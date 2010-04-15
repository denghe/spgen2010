using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
using Smo = Microsoft.SqlServer.Management.Smo;
using Utils = SPGen2010.Components.Helpers.MsSql.Utils;

namespace SPGen2010.Components.Generators.MsSql.Table
{
    class CS_Enum : IGenerator
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
                    this._properties.Add(GenProperties.Name, "CS/Enum/1");
                    this._properties.Add(GenProperties.Caption, "CS：将表的数据转为 Enum");
                    this._properties.Add(GenProperties.Group, "C#");
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
        /// only one primary key, and must be INTEGER (2bytes, 4bytes, 8bytes) type
        /// </summary>
        public bool Validate(params Oe.NodeBase[] targetElements)
        {
            var t = (Smo.Table)targetElements[0].Tag;
            var pks = Utils.GetPrimaryKeyColumns(t);
            return pks.Count == 1 && Utils.CheckIsNumericType(pks[0]);
        }

        #endregion

        public GenResult Generate(params Oe.NodeBase[] targetElements)
        {
            #region Init

            GenResult gr;
            var t = (Smo.Table)targetElements[0].Tag;
            var db = t.Parent;

            var pks = Utils.GetPrimaryKeyColumns(t);
            Smo.Column vc = pks[0], nc = null;

            var sacs = Utils.GetSearchableColumns(t);
            if (sacs.Count == 0)
            {
                nc = vc;
            }
            else
            {
                nc = sacs[0];
            }

            var sb = new StringBuilder();

            #endregion

            #region Gen

            DataSet  ds  = null;
            try
            {
                ds = db.ExecuteWithResults("SELECT [" + Utils.GetEscapeSqlObjectName(vc.Name) + "], [" + Utils.GetEscapeSqlObjectName(nc.Name) + "] FROM [" + Utils.GetEscapeSqlObjectName(t.Schema) + "].[" + Utils.GetEscapeSqlObjectName(t.Name) + @"] ORDER BY [" + Utils.GetEscapeSqlObjectName(nc.Name) + "]");
            }
            catch { }
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                gr = new GenResult(GenResultTypes.Message);
                gr.Message = "当前表中没有数据！生成失败！";
                return gr;
            }

            var tbn = Utils.GetEscapeSqlObjectName(t.Name);
            sb.Append(@"
/// <summary>
/// " + Utils.GetDescription(t) + @"
/// </summary>
public enum " + tbn + @"
{");
            foreach (DataRow c in ds.Tables[0].Rows)
            {
                sb.Append(@"
    " + Utils.GetEscapeName(c[nc.Name].ToString()) + @" = " + c[vc.Name].ToString() + @",");
            }
            sb.Append(@"
}
            ");

            #endregion

            #region return

            gr = new GenResult(GenResultTypes.CodeSegment);
            gr.CodeSegment.first = this.Properties[GenProperties.Tips].ToString();
            gr.CodeSegment.second = sb.ToString();
            return gr;

            //gr = new GenResult(GenResultTypes.CodeSegments);
            //gr.CodeSegments.Add("1", sb.ToString());
            //gr.CodeSegments.Add("2", sb.ToString());
            //return gr;

            //gr = new GenResult(GenResultTypes.Files);
            //gr.Files.Add("1.txt", sb);
            //gr.Files.Add("2.txt", sb);
            //return gr;

            #endregion
        }

    }
}
