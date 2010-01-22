﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
//using Smo = Microsoft.SqlServer.Management.Smo;
//using Utils = SPGen2010.Components.Helpers.MsSql.Utils;
using SPGen2010.Components.Windows;
//using SPGen2010.Components.Providers;

using SPGen2010.Components.Generators.Extensions.CS;

namespace SPGen2010.Components.Generators.MsSql.Database
{
    class DAL : IGenerator
    {
        #region Settings

        public SqlElementTypes TargetSqlElementType
        {
            get { return SqlElementTypes.Database; }
        }
        public Dictionary<GenProperties, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    this._properties = new Dictionary<GenProperties, object>();
                    this._properties.Add(GenProperties.Name, "DAL");
                    this._properties.Add(GenProperties.Caption, "根据 Database 生成 DAL 层");
                    this._properties.Add(GenProperties.Group, "C#");
                    this._properties.Add(GenProperties.Tips, "根据 Database 生成 DAL 层");
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
            return true;
        }

        #endregion

        public GenResult Generate(params Oe.NodeBase[] targetElements)
        {
            #region Init

            var gr = new GenResult(GenResultTypes.Files);
            var oe_db = (Oe.Database)targetElements[0];
            var db = WMain.Instance.MySmoProvider.GetDatabase(oe_db);

            #endregion

            // todo: Get Namespace replace "DAL"

            #region Gen Class Declares

            #region Gen Tables

            var sb = new StringBuilder();

            {
                sb.Append(@"
using System;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    sb.Append(@"
namespace DAL.Tables." + ts.Key.Escape() + @"
{");
                    foreach (var t in ts)
                    {
                        sb.Append(t.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @"
    {");
                        var L = t.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach (var c in t.Columns)
                        {
                            var typename = (c.Nullable ? c.DataType.GetNullableTypeName() : c.DataType.GetTypeName()).FillSpace(10);
                            var fieldname = c.GetEscapeName().FillSpace(L);
                            sb.Append(c.Description.ToSummary(2));
                            sb.Append(@"
        public " + typename + @" " + fieldname + @"{ get; set; }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Tables.cs", sb);
            }

            #endregion

            #region Gen Views

            {
                sb.Clear();

                sb.Append(@"
using System;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach (var vs in schemas)
                {
                    sb.Append(@"
namespace DAL.Views." + vs.Key.Escape() + @"
{");
                    foreach (var v in vs)
                    {
                        sb.Append(v.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @"
    {");
                        var L = v.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach (var c in v.Columns)
                        {
                            var typename = (c.Nullable ? c.DataType.GetNullableTypeName() : c.DataType.GetTypeName()).FillSpace(10);
                            var fieldname = c.GetEscapeName().FillSpace(L);
                            sb.Append(c.Description.ToSummary(2));
                            sb.Append(@"
        public " + typename + @" " + fieldname + @"{ get; set; }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Views.cs", sb);
            }

            #endregion

            #region Gen UserDefinedTableTypes

            {
                sb.Clear();

                sb.Append(@"
using System;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach (var tts in schemas)
                {
                    sb.Append(@"
namespace DAL.UserDefinedTableTypes." + tts.Key.Escape() + @"
{");
                    foreach (var tt in tts)
                    {
                        sb.Append(tt.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @"
    {");
                        var L = tt.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach (var c in tt.Columns)
                        {
                            var typename = (c.Nullable ? c.DataType.GetNullableTypeName() : c.DataType.GetTypeName()).FillSpace(10);
                            var fieldname = c.GetEscapeName().FillSpace(L);
                            sb.Append(c.Description.ToSummary(2));
                            sb.Append(@"
        public " + typename + @" " + fieldname + @"{ get; set; }");
                        }
                        sb.Append(@"
    }
    public partial class " + tt.GetEscapeName() + @"_Collection : List<" + tt.GetEscapeName() + @">
    {
        // todo: ToDataTable()
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region Gen UserDefinedFunctions_Table

            {
                sb.Clear();

                sb.Append(@"
using System;
using UDTT = DAL.UserDefinedTableTypes;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach (var fs in schemas)
                {
                    sb.Append(@"
namespace DAL.UserDefinedFunctions." + fs.Key.Escape() + @"
{");
                    foreach (var f in fs)
                    {
                        sb.Append(f.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @"
    {");
                        sb.Append(@"
        public partial class Parameters
        {");
                        var L = f.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                        foreach (var p in f.Parameters)
                        {
                            var pn = p.GetEscapeName();
                            string pdn;
                            if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                pdn = "UDTT." + p.DataType.GetTypeName() + "_Collection";
                            else pdn = p.DataType.GetNullableTypeName().FillSpace(10);
                            sb.Append(@"
            #region " + pn + @"
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            private " + "bool".FillSpace(10) + @" _f_" + pn + @";
            private " + pdn + @" _v_" + pn + @";
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            public " + pdn + @" " + pn + @"
            {
                get
                {
                    return _v_" + pn + @";
                }
                set
                {
                    _f_" + pn + @" = true;
                    _v_" + pn + @" = value;
                }
            }

            #endregion");
                        }
                        sb.Append(@"
        }
        public partial class ResultTable
        {");
                        L = f.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach (var c in f.Columns)
                        {
                            var typename = (c.Nullable ? c.DataType.GetNullableTypeName() : c.DataType.GetTypeName()).FillSpace(10);
                            var fieldname = c.GetEscapeName().FillSpace(L);
                            sb.Append(c.Description.ToSummary(3));
                            sb.Append(@"
            public " + typename + @" " + fieldname + @"{ get; set; }");
                        }
                        sb.Append(@"
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #region Gen UserDefinedFunctions_Scalar

            {
                sb.Clear();

                sb.Append(@"
using System;
using UDTT = DAL.UserDefinedTableTypes;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Scalar
                              group func by func.Schema;
                foreach (var fs in schemas)
                {
                    sb.Append(@"
namespace DAL.UserDefinedFunctions." + fs.Key.Escape() + @"
{");
                    foreach (var f in fs)
                    {
                        sb.Append(f.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @"
    {");
                        sb.Append(@"
        public partial class Parameters
        {");
                        var L = f.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                        foreach (var p in f.Parameters)
                        {
                            var pn = p.GetEscapeName();
                            string pdn;
                            if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                pdn = "UDTT." + p.DataType.GetTypeName() + "_Collection";
                            else pdn = p.DataType.GetNullableTypeName().FillSpace(10); 
                            sb.Append(@"
            #region " + pn + @"
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            private " + "bool".FillSpace(10) + @" _f_" + pn + @";
            private " + pdn + @" _v_" + pn + @";
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            public " + pdn + @" " + pn + @"
            {
                get
                {
                    return _v_" + pn + @";
                }
                set
                {
                    _f_" + pn + @" = true;
                    _v_" + pn + @" = value;
                }
            }

            #endregion");
                        }
                        sb.Append(@"
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_UserDefinedFunctions_Scalar.cs", sb);
            }

            #endregion

            #region Gen StoredProcedures


            {
                sb.Clear();

                sb.Append(@"
using System;
using UDTT = DAL.UserDefinedTableTypes;
");
                var schemas = from sp in db.StoredProcedures group sp by sp.Schema;
                foreach (var sps in schemas)
                {
                    sb.Append(@"
namespace DAL.StoredProcedures." + sps.Key.Escape() + @"
{");
                    foreach (var sp in sps)
                    {
                        sb.Append(sp.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + sp.GetEscapeName() + @"
    {
        public partial class Parameters
        {");
                        var L = sp.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                        foreach (var p in sp.Parameters)
                        {
                            var pn = p.GetEscapeName();
                            string pdn;
                            if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                pdn = "UDTT." + p.DataType.GetTypeName() + "_Collection";
                            else pdn = p.DataType.GetNullableTypeName().FillSpace(10);
                            sb.Append(@"
            #region " + pn + @"
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            private " + "bool".FillSpace(10) + @" _f_" + pn + @";
            private " + pdn + @" _v_" + pn + @";
");
                            sb.Append(p.Description.ToSummary(3));
                            sb.Append(@"
            public " + pdn + @" " + pn + @"
            {
                get
                {
                    return _v_" + pn + @";
                }
                set
                {
                    _f_" + pn + @" = true;
                    _v_" + pn + @" = value;
                }
            }

            #endregion");
                        }
                        sb.Append(@"
        }

        public partial class ResultSet
        {
            public int ReturnValue { get; set; }
            public ResultTable1 Result1 { get; set; }
            public ResultTable2 Result2 { get; set; }
            //...
        }

        #region Result Class Declares

        public partial class ResultRow1
        {
            //columns properties......
        }
        public partial class ResultRow2
        {
            //columns properties......
        }
        // ...

        public partial class ResultTable1 : List<ResultRow1>
        {
        }
        public partial class ResultTable2 : List<ResultRow2>
        {
        }
        // ...

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_StoredProcedures.cs", sb);
            }


            #endregion

            #endregion

            #region Gen Class Server Extension Methods

            #endregion

            return gr;
        }

    }
}
