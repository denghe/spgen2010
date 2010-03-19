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

namespace SPGen2010.Components.Generators.MsSql.Database {
    class DAL : IGenerator {
        #region Settings

        public SqlElementTypes TargetSqlElementType {
            get { return SqlElementTypes.Database; }
        }
        public Dictionary<GenProperties, object> Properties {
            get {
                if(_properties == null) {
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
        public bool Validate(params Oe.NodeBase[] targetElements) {
            return true;
        }

        #endregion

        public GenResult Generate(params Oe.NodeBase[] targetElements) {
            #region Init

            var gr = new GenResult(GenResultTypes.Files);
            var oe_db = (Oe.Database)targetElements[0];
            var db = WMain.Instance.MySmoProvider.GetDatabase(oe_db);

            #endregion

            // todo: Get Namespace replace "DAL"
            var sb = new StringBuilder();

            #region Gen Database Class

            #region Gen Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from table in db.Tables group table by table.Schema;

                foreach(var ts in schemas) {
                    sb.Append(@"
namespace DAL.Database.Tables." + ts.Key.Escape() + @"
{
");
                    foreach(var t in ts) {
                        sb.Append(t.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @"
    {");
                        var L = t.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach(var c in t.Columns) {
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

                gr.Files.Add("DAL_Database_Tables.cs", sb);
            }

            #endregion

            #region Gen Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach(var vs in schemas) {

                    sb.Append(@"
namespace DAL.Database.Views." + vs.Key.Escape() + @"
{
");
                    foreach(var v in vs) {
                        sb.Append(v.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @"
    {");
                        var L = v.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach(var c in v.Columns) {
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

                gr.Files.Add("DAL_Database_Views.cs", sb);
            }

            #endregion

            #region Gen UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {

                    sb.Append(@"
namespace DAL.Database.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        sb.Append(tt.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @"
    {");
                        var L = tt.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach(var c in tt.Columns) {
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

                gr.Files.Add("DAL_Database_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region Gen UserDefinedFunctions_Table

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using UDTT = DAL.Database.UserDefinedTableTypes;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach(var fs in schemas) {

                    sb.Append(@"
namespace DAL.Database.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        sb.Append(f.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @"
    {");
                        sb.Append(@"
        public partial class Parameters
        {");
                        var L = f.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                        foreach(var p in f.Parameters) {
                            var pn = p.GetEscapeName();
                            string pdn;
                            if(p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
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
                        foreach(var c in f.Columns) {
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

                gr.Files.Add("DAL_Database_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #region Gen UserDefinedFunctions_Scalar

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using UDTT = DAL.Database.UserDefinedTableTypes;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Scalar
                              group func by func.Schema;
                foreach(var fs in schemas) {

                    sb.Append(@"
namespace DAL.Database.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        sb.Append(f.Description.ToSummary(1));
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @"
    {");
                        sb.Append(@"
        public partial class Parameters
        {");
                        var L = f.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                        foreach(var p in f.Parameters) {
                            var pn = p.GetEscapeName();
                            string pdn;
                            if(p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
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

                gr.Files.Add("DAL_Database_UserDefinedFunctions_Scalar.cs", sb);
            }

            #endregion

            #region Gen StoredProcedures

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using UDTT = DAL.Database.UserDefinedTableTypes;
using SqlLib;
");
                var schemas = from sp in db.StoredProcedures group sp by sp.Schema;
                foreach(var sps in schemas) {

                    sb.Append(@"
namespace DAL.Database.StoredProcedures." + sps.Key.Escape() + @"
{
");
                    foreach(var sp in sps) {
                        sb.Append(sp.Description.ToSummary(1));
                        sb.Append(@"
    public static partial class " + sp.GetEscapeName() + @"
    {
");
                        if(sp.Parameters.Count > 0) {
                            sb.Append(@"
        public partial class Parameters
        {");
                            var L = sp.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                            foreach(var p in sp.Parameters) {
                                var pn = p.GetEscapeName();
                                string pdn;
                                if(p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
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
                        } else {
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Database_StoredProcedures.cs", sb);
            }


            #endregion

            #endregion

            #region Gen Expressions Class

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Expressions;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach(var ts in schemas) {

                    sb.Append(@"
namespace DAL.Expressions.Tables." + ts.Key.Escape() + @"
{
");
                    foreach(var t in ts) {
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @" : LogicalNode<" + t.GetEscapeName() + @">
    {");
                        foreach(var c in t.Columns) {
                            var s = (c.Nullable ? "_Nullable_" : "_") + c.DataType.GetExpressionTypeName();
                            var typename = "ExpNode" + s + "<" + t.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New" + s + "(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Expressions_Tables.cs", sb);
            }

            #endregion

            #region Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Expressions;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach(var vs in schemas) {

                    sb.Append(@"
namespace DAL.Expressions.Views." + vs.Key.Escape() + @"
{
");
                    foreach(var v in vs) {
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @" : LogicalNode<" + v.GetEscapeName() + @">
    {");
                        foreach(var c in v.Columns) {
                            var s = (c.Nullable ? "_Nullable_" : "_") + c.DataType.GetExpressionTypeName();
                            var typename = "ExpNode" + s + "<" + v.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New" + s + "(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Expressions_Views.cs", sb);
            }

            #endregion

            #region UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Expressions;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @" : LogicalNode<" + tt.GetEscapeName() + @">
    {");
                        foreach(var c in tt.Columns) {
                            var s = (c.Nullable ? "_Nullable_" : "_") + c.DataType.GetExpressionTypeName();
                            var typename = "ExpNode" + s + "<" + tt.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New" + s + "(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Expressions_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region UserDefinedFunctions_Table

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Expressions;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach(var fs in schemas) {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @" : LogicalNode<" + f.GetEscapeName() + @">
    {");
                        foreach(var c in f.Columns) {
                            var s = (c.Nullable ? "_Nullable_" : "_") + c.DataType.GetExpressionTypeName();
                            var typename = "ExpNode" + s + "<" + f.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New" + s + "(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Expressions_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen Orientations Class

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Orientations;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach(var ts in schemas) {

                    sb.Append(@"
namespace DAL.Orientations.Tables." + ts.Key.Escape() + @"
{
");
                    foreach(var t in ts) {
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @" : LogicalNode<" + t.GetEscapeName() + @">
    {");
                        foreach(var c in t.Columns) {
                            var typename = "ExpNode" + "<" + t.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New_Column(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Orientations_Tables.cs", sb);
            }

            #endregion

            #region Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Orientations;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach(var vs in schemas) {

                    sb.Append(@"
namespace DAL.Orientations.Views." + vs.Key.Escape() + @"
{
");
                    foreach(var v in vs) {
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @" : LogicalNode<" + v.GetEscapeName() + @">
    {");
                        foreach(var c in v.Columns) {
                            var typename = "ExpNode" + "<" + v.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New_Column(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Orientations_Views.cs", sb);
            }

            #endregion

            #region UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Orientations;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @" : LogicalNode<" + tt.GetEscapeName() + @">
    {");
                        foreach(var c in tt.Columns) {
                            var typename = "ExpNode" + "<" + tt.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New_Column(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Orientations_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region UserDefinedFunctions_Table

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Orientations;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach(var fs in schemas) {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @" : LogicalNode<" + f.GetEscapeName() + @">
    {");
                        foreach(var c in f.Columns) {
                            var typename = "ExpNode" + "<" + f.GetEscapeName() + ">";
                            var propertyname = c.GetEscapeName();
                            var methodname = "this.New_Column(@\"" + c.Name.Replace("\"", "\"\"") + "\")";
                            sb.Append(@"
        public " + typename + @" " + propertyname + " { get { return " + methodname + @"; } }");
                        }
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Orientations_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen Queries Class

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Queries;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach(var ts in schemas) {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Tables." + sn + @"
{
");
                    foreach(var t in ts) {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    public partial class " + tn + @" : Query<" + tn + @", Expressions.Tables." + sn + @"." + tn + @", Orientations.Tables." + sn + @"." + tn + @", ColumnEnums.Tables." + sn + @"." + tn + @">
    {
        public override string ToSqlString(string schema = null, string name = null, List<string> columns = null)
        {
            return base.ToSqlString(schema ?? @""" + ts.Key.Replace("\"", "\"\"") + @""", name ?? @""" + t.Name.Replace("\"", "\"\"") + @""", columns);
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Queries_Tables.cs", sb);
            }

            #endregion

            #region Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Queries;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach(var vs in schemas) {
                    var sn = vs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Views." + vs.Key.Escape() + @"
{
");
                    foreach(var v in vs) {
                        var vn = v.GetEscapeName();
                        sb.Append(@"
    public partial class " + vn + @" : Query<" + vn + @", Expressions.Views." + sn + @"." + vn + @", Orientations.Views." + sn + @"." + vn + @", ColumnEnums.Views." + sn + @"." + vn + @">
    {
        public override string ToSqlString(string schema = null, string name = null, List<string> columns = null)
        {
            return base.ToSqlString(schema ?? @""" + vs.Key.Replace("\"", "\"\"") + @""", name ?? @""" + v.Name.Replace("\"", "\"\"") + @""", columns);
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Queries_Views.cs", sb);
            }

            #endregion

            #region UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Queries;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {
                    var sn = tts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        var ttn = tt.GetEscapeName();
                        sb.Append(@"
    public partial class " + ttn + @" : Query<" + ttn + @", Expressions.UserDefinedTableTypes." + sn + @"." + ttn + @", Orientations.UserDefinedTableTypes." + sn + @"." + ttn + @", ColumnEnums.UserDefinedTableTypes." + sn + @"." + ttn + @">
    {
        public override string ToSqlString(string schema = null, string name = null, List<string> columns = null)
        {
            return base.ToSqlString(schema ?? @""" + tts.Key.Replace("\"", "\"\"") + @""", name ?? @""" + tt.Name.Replace("\"", "\"\"") + @""", columns);
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Queries_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region UserDefinedFunctions_Table

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Queries;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach(var fs in schemas) {
                    var sn = fs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        var fn = f.GetEscapeName();
                        sb.Append(@"
    public partial class " + fn + @" : Query<" + fn + @", Expressions.UserDefinedFunctions." + sn + @"." + fn + @", Orientations.UserDefinedFunctions." + sn + @"." + fn + @", ColumnEnums.UserDefinedFunctions." + sn + @"." + fn + @">
    {
        public override string ToSqlString(string schema = null, string name = null, List<string> columns = null)
        {
            return base.ToSqlString(schema ?? @""" + fs.Key.Replace("\"", "\"\"") + @""", name ?? @""" + f.Name.Replace("\"", "\"\"") + @""", columns);
        }
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Queries_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen ColumnEnums Class

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.ColumnEnums;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach(var ts in schemas) {
                    sb.Append(@"
namespace DAL.ColumnEnums.Tables." + ts.Key.Escape() + @"
{
");
                    foreach(var t in ts) {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    public partial class " + tn + @" : ColumnList<" + tn + @">
    {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + tn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if(i < t.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
        };
        public override string GetColumnName(int i) {
            return __cns[i];
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_ColumnEnums_Tables.cs", sb);
            }

            #endregion

            #region Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.ColumnEnums;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach(var vs in schemas) {
                    sb.Append(@"
namespace DAL.ColumnEnums.Views." + vs.Key.Escape() + @"
{
");
                    foreach(var v in vs) {
                        var vn = v.GetEscapeName();
                        sb.Append(@"
    public partial class " + vn + @" : ColumnList<" + vn + @">
    {");
                        for(int i = 0; i < v.Columns.Count; i++) {
                            var c = v.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + vn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for(int i = 0; i < v.Columns.Count; i++) {
                            var c = v.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if(i < v.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
        };
        public override string GetColumnName(int i) {
            return __cns[i];
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_ColumnEnums_Views.cs", sb);
            }

            #endregion

            #region UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.ColumnEnums;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        var ttn = tt.GetEscapeName();
                        sb.Append(@"
    public partial class " + ttn + @" : ColumnList<" + ttn + @">
    {");
                        for(int i = 0; i < tt.Columns.Count; i++) {
                            var c = tt.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + ttn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for(int i = 0; i < tt.Columns.Count; i++) {
                            var c = tt.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if(i < tt.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
        };
        public override string GetColumnName(int i) {
            return __cns[i];
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_ColumnEnums_UserDefinedTableTypes.cs", sb);
            }

            #endregion

            #region UserDefinedFunctions_Table

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.ColumnEnums;
");
                var schemas = from func in db.UserDefinedFunctions
                              where func.FunctionType == MySmo.UserDefinedFunctionType.Table
                              group func by func.Schema;
                foreach(var fs in schemas) {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach(var f in fs) {
                        var fn = f.GetEscapeName();
                        sb.Append(@"
    public partial class " + fn + @" : ColumnList<" + fn + @">
    {");
                        for(int i = 0; i < f.Columns.Count; i++) {
                            var c = f.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + fn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for(int i = 0; i < f.Columns.Count; i++) {
                            var c = f.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if(i < f.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
        };
        public override string GetColumnName(int i) {
            return __cns[i];
        }
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_ColumnEnums_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion



            #region Gen Database Class Server Extension Methods

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach(var ts in schemas) {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Tables." + sn + @"
{
");
                    foreach(var t in ts) {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
");
                        #region Select

                        sb.Append(@"
        public static List<" + tn + @"> Select(Queries.Tables." + sn + @"." + tn + @" q)
        {
            var tsql = q.ToSqlString();
            var rows = new List<" + tn + @">();
            using(var reader = SqlHelper.ExecuteDataReader(tsql))
            {
                var count = q.Columns == null ? 0 : q.Columns.Count();
                if(count > 0) {
                    while(reader.Read()) {
                        var row = new " + tn + @"();
                        for(int i = 0; i < count; i++) {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            ");
                            if(i > 0) sb.Append("else if(i < count && ");
                            else sb.Append("if(");
                            sb.Append(@"q.Contains(" + i + @") {row." + cn + @" = ");
                            if(c.Nullable) {
                                var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                            } else {
                                if(c.DataType.CheckIsBinaryType()) {
                                    sb.Append(@"reader.GetSqlBinary(i).Value");
                                } else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                            }
                            sb.Append(@"; i++; }");
                        }
                        sb.Append(@"
                        }
                        rows.Add(row);
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        rows.Add(new " + tn + @"
                        {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            " + cn + " = ");
                            var s = "";
                            if(c.Nullable) {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                            } else {
                                if(c.DataType.CheckIsBinaryType()) {
                                    sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                } else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                            }
                            if(i < t.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
                        });
                    }
                }

            }
            return rows;
        }

        public static List<" + tn + @"> Select(
            Expressions.Tables." + sn + @"." + tn + @".Handler where = null
            , Orientations.Tables." + sn + @"." + tn + @".Handler orderby = null
            , int pageSize = 0
            , int pageIndex = 0
            , ColumnEnums.Tables." + sn + @"." + tn + @".Handler columns = null
            )
        {
            return Select(Queries.Tables." + sn + @"." + tn + @".New(where, orderby, pageSize, pageIndex, columns));
        }
");
                        if(t.GetPKColumns().Count > 0) {
                            sb.Append(@"
        public static " + tn + @" Select(");
                            var pks = t.GetPKColumns();
                            for(int i = 0; i < pks.Count; i++) {
                                var c = pks[i];
                                var cn = c.Name.Escape();
                                if(i > 0) sb.Append(@", ");
                                sb.Append(c.DataType.GetTypeName() + " c" + i);
                            }
                            sb.Append(@", ColumnEnums.Tables." + sn + @"." + tn + @".Handler columns = null)
        {
            return Select(o => ");
                            for(int i = 0; i < pks.Count; i++) {
                                var c = pks[i];
                                var cn = c.Name.Escape();
                                if(i > 0) sb.Append(@" & ");
                                sb.Append("o." + cn + ".Equal(c" + i + ")");
                            }
                            sb.Append(@", columns: columns).FirstOrDefault();
        }
");
                        }

                        #endregion

                        #region Insert

                        sb.Append(@"
			public static int Insert(" + tn + @" o, ClassEnums.Tables." + sn + @"." + tn + @".Handler h = null)
			{
				var isFirst = true;
				var cmd = new SqlCommand();
				var sb = new StringBuilder(""");
                        var dbtn = "[" + t.Schema.Replace("]", "]]") + @"].[" + t.Name.Replace("]", "]]") + @"]";
                        sb.Append(@"INSERT INTO " + dbtn + @" (");
                        var wcs = t.GetPKColumns();
                        for(int i = 0; i < wcs.Count; i++)
                            sb.Append((i > 0 ? ", " : "") + "[" + wcs[i].Name.Replace("]", "]]") + @"]");
                        sb.Append(@") OUTPUT Inserted.* VALUES (");
                        for(int i = 0; i < wcs.Count; i++)
                            sb.Append((i > 0 ? ", " : "") + "@" + wcs[i].Name.Escape());
                        sb.Append(@");");
                        sb.Append(@"INSERT INTO " + dbtn + @" ("");
				var sb2 = new StringBuilder();");
                        foreach(var c in wcs) {
                            var cn = c.Name.Escape();
                            sb.Append(@"
                var cols = h.Invoke(new ClassEnums.Tables." + sn + @"." + tn + @"());
				if (cols.Contains(" + c.GetOrdinal() + @"))
				{
					cmd.AddParameter(""" + cn + @""", o." + cn + @");
					sb.Append((isFirst ? """" : "", "") + ""[" + cn + @"]"");
					sb2.Append((isFirst ? """" : "", "") + ""@" + cn + @""");
					isFirst = false;
				}");
                        }
                        sb.Append(@"
				sb.Append("") OUTPUT INSERTED.* VALUES ("");
				sb.Append(sb2);
				sb.Append(@"");"");");
                        sb.Append(@"
				cmd.CommandText = sb.ToString();
				return SqlHelper.ExecuteNonQuery(cmd);
			}");

                        #endregion

                        #region Update

                        #endregion

                        #region Delete

                        #endregion

                        // insert
                        // update
                        // delete

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Database_Tables_Methods.cs", sb);
            }

            #endregion

            #region Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Views group table by table.Schema;
                foreach(var ts in schemas) {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Views." + sn + @"
{
");
                    foreach(var t in ts) {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        public static List<" + tn + @"> Select(Queries.Views." + sn + @"." + tn + @" q)
        {
            var tsql = q.ToSqlString();
            var rows = new List<" + tn + @">();
            using(var reader = SqlHelper.ExecuteDataReader(tsql))
            {
                var count = q.Columns == null ? 0 : q.Columns.Count();
                if(count > 0) {
                    while(reader.Read()) {
                        var row = new " + tn + @"();
                        for(int i = 0; i < count; i++) {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            ");
                            if(i > 0) sb.Append("else if(i < count && ");
                            else sb.Append("if(");
                            sb.Append(@"q.Columns[i] == @""" + c.Name.Replace("\"", "\"\"") + @""") {row." + cn + @" = ");
                            var s = "";
                            if(c.Nullable) {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                            } else {
                                if(c.DataType.CheckIsBinaryType()) {
                                    sb.Append(@"reader.GetSqlBinary(i).Value");
                                } else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                            }
                            sb.Append(@"; i++; }");
                        }
                        sb.Append(@"
                        }
                        rows.Add(row);
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        rows.Add(new " + tn + @"
                        {");
                        for(int i = 0; i < t.Columns.Count; i++) {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            " + cn + " = ");
                            var s = "";
                            if(c.Nullable) {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                            } else {
                                if(c.DataType.CheckIsBinaryType()) {
                                    sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                } else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                            }
                            if(i < t.Columns.Count - 1) sb.Append(",");
                        }
                        sb.Append(@"
                        });
                    }
                }

            }
            return rows;
        }

        public static List<" + tn + @"> Select(
            Expressions.Views." + sn + @"." + tn + @".Handler where = null
            , Orientations.Views." + sn + @"." + tn + @".Handler orderby = null
            , int pageSize = 0
            , int pageIndex = 0
            , ColumnEnums.Views." + sn + @"." + tn + @".Handler columns = null
            )
        {
            return Select(Queries.Views." + sn + @"." + tn + @".New(where, orderby, pageSize, pageIndex, columns));
        }
");
                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Database_Views_Methods.cs", sb);
            }

            #endregion

            #region UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Data;
using System.Collections.Generic;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach(var tts in schemas) {

                    sb.Append(@"
namespace DAL.Database.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach(var tt in tts) {
                        sb.Append(tt.Description.ToSummary(1));
                        sb.Append(@"
    partial class " + tt.GetEscapeName() + @"_Collection : List<" + tt.GetEscapeName() + @">
    {
        public DataTable ToDataTable()
        {");
                        var L = tt.Columns.Max(c => c.GetEscapeName().GetByteCount()) + 1;
                        foreach(var c in tt.Columns) {
                            var typename = (c.Nullable ? c.DataType.GetNullableTypeName() : c.DataType.GetTypeName()).FillSpace(10);
                            var fieldname = c.GetEscapeName().FillSpace(L);
                            sb.Append(c.Description.ToSummary(2));
                            //                            sb.Append(@"
                            //        public " + typename + @" " + fieldname + @"{ get; set; }");
                        }
                        sb.Append(@"
            return null;
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Database_UserDefinedTableTypes_Methods.cs", sb);
            }

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
                sb.Append(@"using System;
using System.Data;
using System.Collections.Generic;
using UDTT = DAL.Database.UserDefinedTableTypes;
using SqlLib;
");
                var schemas = from sp in db.StoredProcedures group sp by sp.Schema;
                foreach(var sps in schemas) {

                    sb.Append(@"
namespace DAL.Database.StoredProcedures." + sps.Key.Escape() + @"
{
");
                    foreach(var sp in sps) {
                        sb.Append(sp.Description.ToSummary(1));
                        sb.Append(@"
    partial class " + sp.GetEscapeName() + @"
    {
");
                        if(sp.Parameters.Count > 0) {
                            sb.Append(@"
        partial class Parameters
        {");
                            var L = sp.Parameters.Max(c => c.GetEscapeName().GetByteCount()) + 4;
                            var s = "";
                            var s2 = "";
                            foreach(var p in sp.Parameters) {
                                var pn = p.GetEscapeName();
                                string pdn;
                                if(p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                    pdn = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Collection";
                                else pdn = p.DataType.GetNullableTypeName().FillSpace(10);

                                sb.Append(@"
            public bool Exists_" + pn + @"() { return _f_" + pn + @"; }");

                                // ResetFlags Method Content
                                s += @"
                _f_" + pn + @" = false;";
                                // Parameters
                                if(p.IsOutputParameter) {
                                } else {
                                    s2 += @"
            if( ps.Exists_" + pn + @"() ) cmd.AddParameter(""" + pn + @""", ps." + pn + @", " + p.DataType.SqlDataType.GetSqlDbType(true) + @", " + (p.IsOutputParameter ? "true" : "false") + @");";
                                }
                            }

                            sb.Append(@"

            public void ResetFlags()
            {" + s + @"
            }
        }");
                            sb.Append(@"
        public static DbSet ExecuteDbSet(Parameters ps)
        {
            var cmd = SqlHelper.NewCommand(""" + sp.GetEscapeName() + @""");" + s2 + @"
            return SqlHelper.ExecuteDbSet(cmd);
        }
");
                        } else {
                            sb.Append(@"
        public static DbSet ExecuteDbSet()
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

                gr.Files.Add("DAL_Database_StoredProcedures_Methods.cs", sb);
            }


            #endregion

            #endregion

            return gr;
        }

    }
}
