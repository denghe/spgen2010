using System;
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
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    sb.Append(@"namespace DAL.Tables." + ts.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
");
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

                var schemas = from view in db.Views group view by view.Schema;
                foreach (var vs in schemas)
                {
                    sb.Append(@"
namespace DAL.Views." + vs.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
");
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

                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach (var tts in schemas)
                {
                    sb.Append(@"namespace DAL.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;

");
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

                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach (var fs in schemas)
                {
                    sb.Append(@"namespace DAL.UserDefinedFunctions." + fs.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
    using UDTT = DAL.UserDefinedTableTypes;
");
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
                                pdn = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Collection";
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

                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Scalar
                              group func by func.Schema;
                foreach (var fs in schemas)
                {
                    sb.Append(@"namespace DAL.UserDefinedFunctions." + fs.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
    using UDTT = DAL.UserDefinedTableTypes;
");
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
                                pdn = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Collection";
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

                var schemas = from sp in db.StoredProcedures group sp by sp.Schema;
                foreach (var sps in schemas)
                {
                    sb.Append(@"namespace DAL.StoredProcedures." + sps.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
    using UDTT = DAL.UserDefinedTableTypes;
    using SqlLib;
");
                    foreach (var sp in sps)
                    {
                        sb.Append(sp.Description.ToSummary(1));
                        sb.Append(@"
    public static partial class " + sp.GetEscapeName() + @"
    {
");
                        if (sp.Parameters.Count > 0)
                        {
                            sb.Append(@"
        public partial class Parameters
        {");
                            var L = sp.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                            foreach (var p in sp.Parameters)
                            {
                                var pn = p.GetEscapeName();
                                string pdn;
                                if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                    pdn = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Collection";
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
        }");
                        }
                        else
                        {
                        }
                        sb.Append(@"
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

            #region Tables



            #endregion

            #region Views



            #endregion

            #region UserDefinedTableTypes



            #endregion

            #region UserDefinedFunctions_Table



            #endregion

            #region UserDefinedFunctions_Table



            #endregion

            #region UserDefinedFunctions_Scalar



            #endregion

            #region StoredProcedures

            {
                sb.Clear();

                var schemas = from sp in db.StoredProcedures group sp by sp.Schema;
                foreach (var sps in schemas)
                {
                    sb.Append(@"namespace DAL.StoredProcedures." + sps.Key.Escape() + @"
{
    using System;
    using System.Collections.Generic;
    using UDTT = DAL.UserDefinedTableTypes;
    using SqlLib;
");
                    foreach (var sp in sps)
                    {
                        sb.Append(sp.Description.ToSummary(1));
                        sb.Append(@"
    partial class " + sp.GetEscapeName() + @"
    {
");
                        if (sp.Parameters.Count > 0)
                        {
                            sb.Append(@"
        partial class Parameters
        {");
                            var L = sp.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                            var s = "";
                            var s2 = "";
                            foreach (var p in sp.Parameters)
                            {
                                var pn = p.GetEscapeName();
                                string pdn;
                                if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                    pdn = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Collection";
                                else pdn = p.DataType.GetNullableTypeName().FillSpace(10);

                                sb.Append(@"
            public bool Exists_" + pn + @"() { return _f_" + pn + @"; }");

                                // ResetFlags Method Content
                                s += @"
                _f_" + pn + @" = false;";
                                // Parameters
                                if (p.IsOutputParameter)
                                {
                                }
                                else
                                {
                                    s2 += @"
            if( ps.Exists_" + pn + @"() ) cmd.AddParameter(""" + pn + @""", ps." + pn + @", " + p.DataType.SqlDataType.GetSqlDbType() + @", " + (p.IsOutputParameter ? "true" : "false") + @");";
                                }
                            }

                            sb.Append(@"

            public void ResetFlags()
            {" + s + @"
            }
        }");
                            sb.Append(@"
        public static DbSet Execute(Parameters ps)
        {
            var cmd = SqlHelper.NewCommand(""" + sp.GetEscapeName() + @""");" + s2 + @"
            return SqlHelper.ExecuteDbSet(cmd);
        }
");
                        }
                        else
                        {
                            sb.Append(@"
        public static DbSet Execute()
        {
            return SqlHelper.ExecuteDbSet(""" + sp.GetEscapeName() + @""", true);
        }
");
                        }



                        // todo: 读 SP 的配置，视情况生成相应 Result 的结构
                        // 先生成执行后返回 DbResult 的方法

                        /*
sb.Append(@"
        public partial class Result : DbSet // ????
        {
        }
        // ...
    }");
                         */

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_StoredProcedures_Methods.cs", sb);
            }


            #endregion

            #endregion

            return gr;
        }

    }
}
