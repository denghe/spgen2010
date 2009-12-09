//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;

//using MySmo = SPGen2010.Components.Modules.MySmo;
//using SmoUtils = SPGen2010.Components.Utils.MsSql.Utils;
//using Smo = Microsoft.SqlServer.Management.Smo;
//using SPGen2010.Components.Fillers;

//namespace SPGen2010.Components.Generators.MsSql.Table
//{
//    class Enum : IGenerator
//    {
//        #region Init

//        public Enum()
//        {
//            this._properties.Add(GenProperties.Name, "Table Enum");
//            this._properties.Add(GenProperties.Caption, "根据单主鍵（整型）表的数据生成 Enum");
//            this._properties.Add(GenProperties.Group, "C#");
//            this._properties.Add(GenProperties.Tips, "为 Table 里面的数据生成 C# 的枚举代码");
//        }
//        public SqlElementTypes TargetSqlElementType
//        {
//            get { return SqlElementTypes.Table; }
//        }

//        #endregion

//        #region Misc

//        private Dictionary<GenProperties, object> _properties = new Dictionary<GenProperties, object>();
//        public Dictionary<GenProperties, object> Properties
//        {
//            get
//            {
//                return this._properties;
//            }
//        }

//        public event System.ComponentModel.CancelEventHandler OnProcessing;

//        private IObjectExplorerFiller _filler;
//        public IObjectExplorerFiller Filler
//        {
//            set { _filler = value; }
//        }

//        private MySmo.Database _db;
//        public MySmo.Database Database
//        {
//            set { _db = value; }
//        }

//        #endregion

//        /// <summary>
//        /// 通过条件：只有一个数字类型主键
//        /// </summary>
//        public bool Validate(params object[] sqlElements)
//        {
//            var mt = (MySmo.Table)sqlElements[0];
//            var mssql_filler = 
            

//            bool b = false;
//            List<Column> pks = SmoUtils.GetPrimaryKeyColumns(t);
//            if (pks.Count == 1)
//            {
//                if (SmoUtils.CheckIsNumericType(pks[0])) b = true;
//            }
//            return b;
//        }

//        public GenResult Gen(params object[] sqlElements)
//        {
//            #region Init

//            GenResult gr;
//            Table t = (Table)sqlElements[0];

//            List<Column> pks = SmoUtils.GetPrimaryKeyColumns(t);

//            if (pks.Count == 0)
//            {
//                gr = new GenResult(GenResultTypes.Message);
//                gr.Message = "无法为没有主键字段的表生成该代码！";
//                return gr;
//            }
//            else if (pks.Count > 1)
//            {
//                gr = new GenResult(GenResultTypes.Message);
//                gr.Message = "无法为多主键字段的表生成该代码！";
//                return gr;
//            }
//            else if (!SmoUtils.CheckIsNumericType(pks[0]))
//            {
//                gr = new GenResult(GenResultTypes.Message);
//                gr.Message = "无法为非数字型主键字段的表生成该代码！";
//                return gr;
//            }

//            Column vc = pks[0];
//            Column nc = null;

//            List<Column> sacs = SmoUtils.GetSearchableColumns(t);
//            if (sacs.Count == 0)
//            {
//                nc = vc;
//            }
//            else
//            {
//                nc = sacs[0];
//            }

//            StringBuilder sb = new StringBuilder();

//            #endregion

//            #region Gen

//            string tbn = SmoUtils.GetEscapeSqlObjectName(t.Name);

//            sb.Append(@"/// <summary>
///// " + SmoUtils.GetDescription(t) + @"
///// </summary>
//public enum " + tbn + @"
//{");
//            DataSet ds = _db.ExecuteWithResults("SELECT [" + SmoUtils.GetEscapeSqlObjectName(vc.Name) + "], [" + SmoUtils.GetEscapeSqlObjectName(nc.Name) + "] FROM [" + SmoUtils.GetEscapeSqlObjectName(t.Schema) + "].[" + SmoUtils.GetEscapeSqlObjectName(t.Name) + @"] ORDER BY [" + SmoUtils.GetEscapeSqlObjectName(nc.Name) + "]");
//            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
//            {
//                gr = new GenResult(GenResultTypes.Message);
//                gr.Message = "当前表中没有数据！生成失败！";
//                return gr;
//            }

//            foreach (DataRow c in ds.Tables[0].Rows)
//            {
//                sb.Append(@"
//	" + SmoUtils.GetEscapeName(c[nc.Name].ToString()) + @" = " + c[vc.Name].ToString() + @",");
//            }
//            sb.Append(@"
//}
//");

//            #endregion

//            #region return

//            gr = new GenResult(GenResultTypes.CodeSegment);
//            gr.CodeSegment = new KeyValuePair<string, string>(this._properties[GenProperties.Tips].ToString(), sb.ToString());
//            return gr;

//            #endregion
//        }
//    }
//}
