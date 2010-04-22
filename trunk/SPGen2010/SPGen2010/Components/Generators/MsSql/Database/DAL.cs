using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using SPGen2010.Components.Windows;
using SPGen2010.Components.Generators.Extensions.Generic;
using SPGen2010.Components.Generators.Extensions.CS;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;


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
                    this._properties.Add(GenProperties.Name, "C#/DAL/1");
                    this._properties.Add(GenProperties.Caption, "C#：根据 Database 生成 DAL 层");
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
            var sb = new StringBuilder();

            #region Gen Database Class

            #region Gen Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from table in db.Tables group table by table.Schema;

                foreach (var ts in schemas)
                {
                    sb.Append(@"
namespace DAL.Database.Tables." + ts.Key.Escape() + @"
{
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

                gr.Files.Add("DAL_Class_Database_Tables.cs", sb);
            }

            #endregion

            #region Gen Views

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from view in db.Views group view by view.Schema;
                foreach (var vs in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.Views." + vs.Key.Escape() + @"
{
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

                gr.Files.Add("DAL_Class_Database_Views.cs", sb);
            }

            #endregion

            #region Gen UserDefinedTableTypes

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
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
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Class_Database_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.UserDefinedFunctions." + fs.Key.Escape() + @"
{
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
                                pdn = "List<UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + ">";
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

                gr.Files.Add("DAL_Class_Database_UserDefinedFunctions_Table.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.UserDefinedFunctions." + fs.Key.Escape() + @"
{
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
                                pdn = "List<UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + ">";
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

                gr.Files.Add("DAL_Class_Database_UserDefinedFunctions_Scalar.cs", sb);
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
                foreach (var sps in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.StoredProcedures." + sps.Key.Escape() + @"
{
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
                                    pdn = "List<UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + ">";
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

                gr.Files.Add("DAL_Class_Database_StoredProcedures.cs", sb);
            }


            #endregion

            #endregion

            #region Gen Database Methods

            #region Tables

            #region Serial Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Tables." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @" : ISerial
    {
");
                        #region Constructor

                        sb.Append(@"
        #region Constructor

        public " + tn + @"() {
        }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
");

                        #endregion

                        #region Serial


                        sb.Append(@"
        #region Serial
        public byte[] GetBytes()
        {
            return new byte[][]
            {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
                this." + c.GetEscapeName() + @".GetBytes(),");
                        }
                        sb.Append(@"
            }.Combine();
        }
        public void Fill(byte[] buffer, ref int startIndex) {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
            this." + c.GetEscapeName() + @" = buffer." + c.DataType.GetToTypeMethod(c.Nullable) + @"(ref startIndex);");
                        }
                        sb.Append(@"
        }
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Database_Tables.cs", sb);
            }
            #endregion

            #region Sql Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Tables." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
");
                        var dbtn = "[" + t.Schema.Replace("]", "]]") + @"].[" + t.Name.Replace("]", "]]") + @"]";
                        var wcs = t.GetWriteableColumns();

                        #region Select

                        sb.Append(@"
        #region Select

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
                        var cols = q.Columns;
                        for(int i = 0; i < count; i++) {");
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            ");
                            if (i > 0) sb.Append("else if(i < count && ");
                            else sb.Append("if(");
                            sb.Append(@"cols.Contains(" + i + @")) {row." + cn + @" = ");
                            if (c.Nullable)
                            {
                                var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                            }
                            else
                            {
                                if (c.DataType.CheckIsBinaryType())
                                {
                                    sb.Append(@"reader.GetSqlBinary(i).Value");
                                }
                                else
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
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            " + cn + " = ");
                            var s = "";
                            if (c.Nullable)
                            {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                            }
                            else
                            {
                                if (c.DataType.CheckIsBinaryType())
                                {
                                    sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                }
                                else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                            }
                            if (i < t.Columns.Count - 1) sb.Append(",");
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
                        if (t.GetPrimaryKeyColumns().Count > 0)
                        {
                            sb.Append(@"
        public static " + tn + @" Select(");
                            var pks = t.GetPrimaryKeyColumns();
                            for (int i = 0; i < pks.Count; i++)
                            {
                                var c = pks[i];
                                var cn = c.Name.Escape();
                                if (i > 0) sb.Append(@", ");
                                sb.Append(c.DataType.GetTypeName() + " c" + i);
                            }
                            sb.Append(@", ColumnEnums.Tables." + sn + @"." + tn + @".Handler columns = null)
        {
            return Select(o => ");
                            for (int i = 0; i < pks.Count; i++)
                            {
                                var c = pks[i];
                                var cn = c.Name.Escape();
                                if (i > 0) sb.Append(@" & ");
                                sb.Append("o." + cn + ".Equal(c" + i + ")");
                            }
                            sb.Append(@", columns: columns).FirstOrDefault();
        }
");
                        }


                        var fks = t.ForeignKeys;
                        if (fks.Count > 0)
                        {
                            foreach (var fk in fks)
                            {
                                var fkt = t.ParentDatabase.Tables.Find(o => o.Name == fk.ReferencedTable && o.Schema == fk.ReferencedTableSchema);
                                var pks = fkt.GetPrimaryKeyColumns();
                                var fktn = fkt.GetEscapeName();
                                var s = "";
                                for (var i = 0; i < pks.Count; i++)
                                {
                                    var fkcn = fk.Columns[i].GetEscapeName();
                                    var pkcn = pks[i].GetEscapeName();
                                    if (i > 0) s += " & ";
                                    s += "o." + fkcn + " == parent." + pkcn;
                                }
                                sb.Append(@"
        public static List<" + tn + @"> Select(" + fktn + @" parent, Queries.Tables." + sn + @"." + tn + @".Handler query = null) {
            if(query == null) return " + tn + @".Select(where: o => " + s + @");
            var q = query(new Queries.Tables." + sn + @"." + tn + @"());
            if(q.Where == null) q.SetWhere(o => " + s + @");
            else q.Where.And(o => " + s + @");
            return " + tn + @".Select(q);
        }
");
                            }
                        }

                        sb.Append(@"
        #endregion
");

                        #endregion

                        #region Insert

                        sb.Append(@"
        #region Insert
");
                        if (db.CompatibilityLevel >= SPGen2010.Components.Modules.MySmo.CompatibilityLevel.Version90)
                        {
                            if (t.TriggersCount == 0)
                            {
                                // 无 trigger, 直接用 output 输出回填

                                #region Implement

                                sb.Append(@"
		public static int Insert(" + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @" ics, ColumnEnums.Tables." + sn + @"." + tn + @" fcs = null, bool isFillAfterInsert = true)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder(@""
INSERT INTO " + dbtn + @" ("");
			var sb2 = new StringBuilder();
			var isFirst = true;
            var fccount = fcs == null ? 0 : fcs.Count();");
                                foreach (var c in wcs)
                                {
                                    var cn = c.Name.Escape();
                                    sb.Append(@"
			if (ics == null || ics.Contains(" + c.GetOrdinal() + @"))
			{");
                                    if (c.Nullable) sb.Append(@"
                var p = new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, null, """", """", """");
                if (o." + cn + @" == null) p.Value = DBNull.Value; else p.Value = o." + cn + @";
                cmd.Parameters.Add(p);");
                                    else sb.Append(@"
                cmd.Parameters.Add(new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, o." + cn + @", """", """", """"));");
                                    sb.Append(@"
				sb.Append((isFirst ? @""
       "" : @""
     , "") + ""[" + cn + @"]"");
				sb2.Append((isFirst ? @""
       "" : @""
     , "") + ""@" + cn + @""");
				isFirst = false;
			}");
                                }
                                sb.Append(@"
            if(isFillAfterInsert)
            {
                if(fcs == null)
                {
                    sb.Append(@""
) 
OUTPUT INSERTED.* VALUES ("");
                }
                else
                {
                    sb.Append(@""
) 
OUTPUT "");
                    for(int i = 0; i < fccount; i++)
                    {
                        if(i > 0) sb.Append(@"", "");
                        sb.Append(@""INSERTED.["" + fcs.GetColumnName(i).Replace(""]"", ""]]"") + ""]"");
                    }
                    sb.Append(@"" VALUES ("");
                }
            }
            else sb.Append(@""
) 
VALUES ("");
			sb.Append(sb2);
			sb.Append(@""
);"");
			cmd.CommandText = sb.ToString();
            if(!isFillAfterInsert)
                return SqlHelper.ExecuteNonQuery(cmd);

            using(var reader = SqlHelper.ExecuteDataReader(cmd))
            {
                if(fccount == 0)
                {
                    while(reader.Read())
                    {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                        o." + cn + " = ");
                                    var s = "";
                                    if (c.Nullable)
                                    {
                                        s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                        sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                                    }
                                    sb.Append(";");
                                }
                                sb.Append(@"
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        for(int i = 0; i < fccount; i++)
                        {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                            ");
                                    if (i > 0) sb.Append("else if(i < fccount && ");
                                    else sb.Append("if(");
                                    sb.Append(@"fcs.Contains(" + i + @")) {o." + cn + @" = ");
                                    if (c.Nullable)
                                    {
                                        var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                        sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(i).Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                                    }
                                    sb.Append(@"; i++; }");
                                }
                                sb.Append(@"
                        }
                    }
                }
                return reader.RecordsAffected;
            }
		}

		public static int Insert(" + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @".Handler insertCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterInsert = true)
		{
            return Insert(o,
                insertCols == null ? null : insertCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                fillCols == null ? null : fillCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                isFillAfterInsert
            );
        }
");
                                #endregion
                            }
                            else
                            {
                                // 有 trigger 的情况下只能将 OUTPUT 输出到一个临时表再输出

                                #region Implement

                                sb.Append(@"
		public static int Insert(" + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @" ics, ColumnEnums.Tables." + sn + @"." + tn + @" fcs = null, bool isFillAfterInsert = true)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();
			var sb2 = new StringBuilder();
			var isFirst = true;
            var fccount = fcs == null ? 0 : fcs.Count();
            if(isFillAfterInsert)
            {
                sb.Append(@""
DECLARE @t TABLE("");
                if(fcs == null)
                {
                    sb.Append(@""");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    sb.Append(@"
    [" + c.Name.Replace("]", "]]") + "] " + c.DataType.ToString());
                                    sb.Append(c.Nullable ? @" NULL" : @" NOT NULL");
                                    if (i < t.Columns.Count - 1) sb.Append(@",");
                                }
                                sb.Append(@""");
                }
                else
                {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    sb.Append(@"
                    if (fcs.Contains(" + c.GetOrdinal() + @")) sb.Append(@""
    [" + c.Name.Replace("]", "]]") + "] " + c.DataType.ToString());
                                    sb.Append(c.Nullable ? @" NULL" : @" NOT NULL");
                                    if (i < t.Columns.Count - 1) sb.Append(@",");
                                    sb.Append(@""");");
                                }
                                sb.Append(@"
                }
                sb.Append(@""
);"");
            }
            sb.Append(@""
INSERT INTO " + dbtn + @" ("");");
                                foreach (var c in wcs)
                                {
                                    var cn = c.Name.Escape();
                                    sb.Append(@"
			if (ics == null || ics.Contains(" + c.GetOrdinal() + @"))
			{");
                                    if (c.Nullable) sb.Append(@"
                var p = new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, null, """", """", """");
                if (o." + cn + @" == null) p.Value = DBNull.Value; else p.Value = o." + cn + @";
                cmd.Parameters.Add(p);");
                                    else sb.Append(@"
                cmd.Parameters.Add(new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, o." + cn + @", """", """", """"));");
                                    sb.Append(@"
				sb.Append((isFirst ? @""
       "" : @""
     , "") + ""[" + cn + @"]"");
				sb2.Append((isFirst ? @""
       "" : @""
     , "") + ""@" + cn + @""");
				isFirst = false;
			}");
                                }
                                sb.Append(@"
            if(isFillAfterInsert)
            {
                if(fcs == null)
                {
                    sb.Append(@""
) 
OUTPUT INSERTED.* INTO @t
VALUES ("");
                }
                else
                {
                    sb.Append(@""
) 
OUTPUT "");
                    for(int i = 0; i < fccount; i++) 
                    {
                        if(i > 0) sb.Append(@"", "");
                        sb.Append(@""INSERTED.["" + fcs.GetColumnName(i).Replace(""]"", ""]]"") + ""]"");
                    }
                    sb.Append(@"" INTO @t
VALUES ("");
                }
            }
            else sb.Append(@""
) 
VALUES ("");
			sb.Append(sb2);
			sb.Append(@""
);"");
            if(isFillAfterInsert) sb.Append(@""
SELECT * FROM @t;"");
			cmd.CommandText = sb.ToString();
            if(!isFillAfterInsert)
                return SqlHelper.ExecuteNonQuery(cmd);

            using(var reader = SqlHelper.ExecuteDataReader(cmd))
            {
                if(fccount == 0)
                {
                    while(reader.Read())
                    {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                        o." + cn + " = ");
                                    var s = "";
                                    if (c.Nullable)
                                    {
                                        s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                        sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                                    }
                                    sb.Append(";");
                                }
                                sb.Append(@"
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        for(int i = 0; i < fccount; i++)
                        {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                            ");
                                    if (i > 0) sb.Append("else if(i < fccount && ");
                                    else sb.Append("if(");
                                    sb.Append(@"fcs.Contains(" + i + @")) {o." + cn + @" = ");
                                    if (c.Nullable)
                                    {
                                        var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                        sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(i).Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                                    }
                                    sb.Append(@"; i++; }");
                                }
                                sb.Append(@"
                        }
                    }
                }
                return reader.RecordsAffected;
            }
		}

		public static int Insert(" + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @".Handler insertCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterInsert = true)
		{
            return Insert(o,
                insertCols == null ? null : insertCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                fillCols == null ? null : fillCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                isFillAfterInsert
            );
        }
");
                                #endregion
                            }
                        }
                        else
                        {
                            // todo
                            // SQL2000 不支持 OUTPUT

                            var pks = t.GetPrimaryKeyColumns();
                            if (pks.Count > 0)
                            {

                                if (pks.Count == 1)
                                {
                                    if (pks[0].Identity)
                                    {
                                        // todo: 自增
                                    }
                                    else if (pks[0].DataType.SqlDataType == MySmo.SqlDataType.UniqueIdentifier
                                        && pks[0].DefaultConstraint.Text.Equals("newid()", StringComparison.OrdinalIgnoreCase)
                                    )
                                    {
                                        // todo: GUID
                                    }
                                }
                                else
                                {
                                    // todo: 多主键情况下, 其中非自增或newid()主键必须传值
                                    // todo: 判断哪个主键是自增或GUID, 做相应处理

                                }
                            }
                            else
                            {
                                // todo: 没主键, 无法回填
                            }
                        }
                        sb.Append(@"
        #endregion
");
                        #endregion

                        #region Update

                        sb.Append(@"
        #region Update
");

                        if (db.CompatibilityLevel >= SPGen2010.Components.Modules.MySmo.CompatibilityLevel.Version90)
                        {
                            if (t.TriggersCount == 0)
                            {
                                // 无 trigger, 直接用 output 输出回填

                                #region Implement

                                sb.Append(@"
		public static int Update(" + tn + @" o, Expressions.Tables." + sn + @"." + tn + @" eh = null, ColumnEnums.Tables." + sn + @"." + tn + @" ucs = null, ColumnEnums.Tables." + sn + @"." + tn + @" fcs = null, bool isFillAfterUpdate = true)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder(@""");
                                sb.Append(@"
UPDATE " + dbtn + @"
   SET ");
                                sb.Append(@""");
			var isFirst = true;
            var fccount = fcs == null ? 0 : fcs.Count();");
                                foreach (var c in wcs)
                                {
                                    var cn = c.Name.Escape();
                                    sb.Append(@"
			if (ucs == null || ucs.Contains(" + c.GetOrdinal() + @"))
			{");
                                    if (c.Nullable) sb.Append(@"
                var p = new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, null, """", """", """");
                if (o." + cn + @" == null) p.Value = DBNull.Value; else p.Value = o." + cn + @";
                cmd.Parameters.Add(p);");
                                    else sb.Append(@"
                cmd.Parameters.Add(new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, o." + cn + @", """", """", """"));");
                                    sb.Append(@"
				sb.Append((isFirst ? @"""" : @""
     , "") + ""[" + cn + @"] = @" + cn + @""");
				isFirst = false;
			}");
                                }
                                sb.Append(@"
            if(isFillAfterUpdate) {
                if(fcs == null) {
                    sb.Append(@""
OUTPUT INSERTED.*"");
                }
                else {
                    sb.Append(@""
OUTPUT "");
                    for(int i = 0; i < fccount; i++) {
                        if(i > 0) sb.Append(@"", "");
                        sb.Append(@""INSERTED.["" + fcs.GetColumnName(i).Replace(""]"", ""]]"") + ""]"");
                    }
                }
            }

            if (eh != null)
            {
                var ws = eh.ToString();
                if(ws.Length > 0)
    			    sb.Append(@""
 WHERE "" + ws);
            }");
                                sb.Append(@"
			cmd.CommandText = sb.ToString();
			if (!isFillAfterUpdate)
                return SqlHelper.ExecuteNonQuery(cmd);

            using(var reader = SqlHelper.ExecuteDataReader(cmd))
            {
                if(fccount == 0)
                {
                    while(reader.Read())
                    {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                        o." + cn + " = ");
                                    var s = "";
                                    if (c.Nullable)
                                    {
                                        s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                        sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                                    }
                                    sb.Append(";");
                                }
                                sb.Append(@"
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        for(int i = 0; i < fccount; i++)
                        {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                            ");
                                    if (i > 0) sb.Append("else if(i < fccount && ");
                                    else sb.Append("if(");
                                    sb.Append(@"fcs.Contains(" + i + @")) {o." + cn + @" = ");
                                    if (c.Nullable)
                                    {
                                        var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                        sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(i).Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                                    }
                                    sb.Append(@"; i++; }");
                                }
                                sb.Append(@"
                        }
                    }
                }
                return reader.RecordsAffected;
            }
            
		}
        public static int Update(" + tn + @" o, Expressions.Tables." + sn + @"." + tn + @".Handler eh = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler updateCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterUpdate = true)
        {
            return Update(o,
                eh == null ? null : eh(new Expressions.Tables." + sn + @"." + tn + @"()),
                updateCols == null ? null : updateCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                fillCols == null ? null : fillCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                isFillAfterUpdate
            );
        }");

                                #endregion

                            }
                            else
                            {
                                // 有 trigger 的情况下只能将 OUTPUT 输出到一个临时表再输出

                                #region Implement

                                sb.Append(@"
		public static int Update(" + tn + @" o, Expressions.Tables." + sn + @"." + tn + @" eh = null, ColumnEnums.Tables." + sn + @"." + tn + @" ucs = null, ColumnEnums.Tables." + sn + @"." + tn + @" fcs = null, bool isFillAfterUpdate = true)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();
			var sb2 = new StringBuilder();
			var isFirst = true;
            var fccount = fcs == null ? 0 : fcs.Count();
            if(isFillAfterUpdate)
            {
                sb.Append(@""
DECLARE @t TABLE("");
                if(fcs == null)
                {
                    sb.Append(@""");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    sb.Append(@"
    [" + c.Name.Replace("]", "]]") + "] " + c.DataType.ToString());
                                    sb.Append(c.Nullable ? @" NULL" : @" NOT NULL");
                                    if (i < t.Columns.Count - 1) sb.Append(@",");
                                }
                                sb.Append(@""");
                }
                else
                {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    sb.Append(@"
                    if (fcs.Contains(" + c.GetOrdinal() + @")) sb.Append(@""
    [" + c.Name.Replace("]", "]]") + "] " + c.DataType.ToString());
                                    sb.Append(c.Nullable ? @" NULL" : @" NOT NULL");
                                    if (i < t.Columns.Count - 1) sb.Append(@",");
                                    sb.Append(@""");");
                                }
                                sb.Append(@"
                }
                sb.Append(@""
);"");
            }
            sb.Append(@""
UPDATE " + dbtn + @"
   SET ");
                                sb.Append(@""");
");
                                foreach (var c in wcs)
                                {
                                    var cn = c.Name.Escape();
                                    sb.Append(@"
			if (ucs == null || ucs.Contains(" + c.GetOrdinal() + @"))
			{");
                                    if (c.Nullable) sb.Append(@"
                var p = new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, null, """", """", """");
                if (o." + cn + @" == null) p.Value = DBNull.Value; else p.Value = o." + cn + @";
                cmd.Parameters.Add(p);");
                                    else sb.Append(@"
                cmd.Parameters.Add(new SqlParameter(""" + cn + @""", " + c.DataType.SqlDataType.GetSqlDbType(true) + @", 0, ParameterDirection.Input, 0, 0, """ + cn + @""", DataRowVersion.Current, false, o." + cn + @", """", """", """"));");
                                    sb.Append(@"
				sb.Append((isFirst ? @"""" : @""
     , "") + ""[" + cn + @"] = @" + cn + @""");
				isFirst = false;
			}");
                                }
                                sb.Append(@"
            if(isFillAfterUpdate) {
                if(fcs == null) {
                    sb.Append(@""
OUTPUT INSERTED.* INTO @t"");
                }
                else
                {
                    sb.Append(@""
OUTPUT "");
                    for(int i = 0; i < fccount; i++)
                    {
                        if(i > 0) sb.Append(@"", "");
                        sb.Append(@""INSERTED.["" + fcs.GetColumnName(i).Replace(""]"", ""]]"") + ""]"");
                    }
                    sb.Append(@"" INTO @t"");
                }
            }

            if (eh != null)
            {
                var ws = eh.ToString();
                if(ws.Length > 0)
    			    sb.Append(@""
 WHERE "" + ws);
            }");
                                sb.Append(@"
            if(isFillAfterUpdate) sb.Append(@""
SELECT * FROM @t;"");
			cmd.CommandText = sb.ToString();
			if (!isFillAfterUpdate)
                return SqlHelper.ExecuteNonQuery(cmd);

            using(var reader = SqlHelper.ExecuteDataReader(cmd))
            {
                if(fccount == 0)
                {
                    while(reader.Read())
                    {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                        o." + cn + " = ");
                                    var s = "";
                                    if (c.Nullable)
                                    {
                                        s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                        sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                                    }
                                    sb.Append(";");
                                }
                                sb.Append(@"
                    }
                }
                else
                {
                    while(reader.Read())
                    {
                        for(int i = 0; i < fccount; i++)
                        {");
                                for (int i = 0; i < t.Columns.Count; i++)
                                {
                                    var c = t.Columns[i];
                                    var cn = c.GetEscapeName();
                                    sb.Append(@"
                            ");
                                    if (i > 0) sb.Append("else if(i < fccount && ");
                                    else sb.Append("if(");
                                    sb.Append(@"fcs.Contains(" + i + @")) {o." + cn + @" = ");
                                    if (c.Nullable)
                                    {
                                        var s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                        sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                                    }
                                    else
                                    {
                                        if (c.DataType.CheckIsBinaryType())
                                        {
                                            sb.Append(@"reader.GetSqlBinary(i).Value");
                                        }
                                        else
                                            sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(i)");
                                    }
                                    sb.Append(@"; i++; }");
                                }
                                sb.Append(@"
                        }
                    }
                }
                return reader.RecordsAffected;
            }
            
		}
        public static int Update(" + tn + @" o, Expressions.Tables." + sn + @"." + tn + @".Handler eh = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler updateCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterUpdate = true)
        {
            return Update(o,
                eh == null ? null : eh(new Expressions.Tables." + sn + @"." + tn + @"()),
                updateCols == null ? null : updateCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                fillCols == null ? null : fillCols(new ColumnEnums.Tables." + sn + @"." + tn + @"()),
                isFillAfterUpdate
            );
        }");

                                #endregion

                            }
                        }
                        else
                        {
                            // todo
                            // SQL2000 不支持 OUTPUT

                            var pks = t.GetPrimaryKeyColumns();
                            if (pks.Count > 0)
                            {
                                // todo: 根据当前主键值回填
                            }
                            else
                            {
                                // todo: 没主键, 无法回填
                            }
                        }

                        sb.Append(@"
        #endregion
");

                        #endregion

                        #region Delete

                        sb.Append(@"
        #region Delete

		public static int Delete(Expressions.Tables." + sn + @"." + tn + @" eh)
		{
			var s = @""");
                        sb.Append(@"
DELETE FROM " + dbtn + @""";");
                        sb.Append(@"
            if (eh != null)
            {
                var ws = eh.ToString();
                if(ws.Length > 0)
    			    s += @""
 WHERE "" + ws;
            }
			return SqlHelper.ExecuteNonQuery(s);
		}
        public static int Delete(Expressions.Tables." + sn + @"." + tn + @".Handler eh)
        {
            return Delete(eh(new Expressions.Tables." + sn + @"." + tn + @"()));
        }");

                        sb.Append(@"
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Sql_Database_Tables.cs", sb);
            }
            #endregion

            #region Sql Extensions Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Tables." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    public static partial class " + tn + @"_Extensions
    {
");
                        var dbtn = "[" + t.Schema.Replace("]", "]]") + @"].[" + t.Name.Replace("]", "]]") + @"]";
                        var wcs = t.GetWriteableColumns();

                        #region Insert

                        sb.Append(@"
        #region Insert

		public static int Insert(this " + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @".Handler insertCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterInsert = true)
		{
            return " + tn + @".Insert(o, insertCols, fillCols, isFillAfterInsert);
		}");

                        sb.Append(@"
        #endregion
");
                        #endregion

                        #region Update

                        sb.Append(@"
        #region Update

		public static int Update(this " + tn + @" o, Expressions.Tables." + sn + @"." + tn + @".Handler eh = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler updateCols = null, ColumnEnums.Tables." + sn + @"." + tn + @".Handler fillCols = null, bool isFillAfterUpdate = true)
		{
            return " + tn + @".Update(o, eh, updateCols, fillCols, isFillAfterUpdate);
		}");

                        sb.Append(@"
        #endregion
");

                        #endregion

                        #region Delete

                        sb.Append(@"
        #region Delete

		public static int Delete(this " + tn + @" o, ColumnEnums.Tables." + sn + @"." + tn + @".Handler conditionCols = null)
		{
            if(conditionCols == null) return " + sn + @"." + tn + @".Delete(t =>");
                        var pkcs = t.GetPrimaryKeyColumns();
                        var ccs = t.GetCompareableColumns();
                        if (pkcs.Count > 0)
                        {
                            for (int i = 0; i < pkcs.Count; i++)
                            {
                                var c = pkcs[i];
                                var cn = c.GetEscapeName();
                                sb.Append(@"
                t." + cn + @" == o." + cn + @"");
                                if (i < pkcs.Count - 1) sb.Append(" &");
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ccs.Count; i++)
                            {
                                var c = ccs[i];
                                var cn = c.GetEscapeName();
                                sb.Append(@"
                t." + cn + @" == o." + cn + @"");
                                if (i < ccs.Count - 1) sb.Append(" &");
                            }
                        }
                        sb.Append(@"
            );
            var cols = conditionCols(new DAL.ColumnEnums.Tables." + sn + @"." + tn + @"());
            var exp = new DAL.Expressions.Tables." + sn + @"." + tn + @"();");

                        for (int i = 0; i < ccs.Count; i++)
                        {
                            var c = ccs[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
            if(cols.Contains(" + c.GetOrdinal() + ")) exp.And(t => t." + cn + @" == o." + cn + @");");
                        }
                        sb.Append(@"
            return " + sn + @"." + tn + @".Delete(exp);
		}
");

                        sb.Append(@"
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Sql_Ext_Database_Tables.cs", sb);
            }

            #endregion

            #endregion

            #region Views

            #region Serial Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SqlLib;
");
                var schemas = from table in db.Views group table by table.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Views." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @" : ISerial
    {");
                        #region Constructor

                        sb.Append(@"
        #region Constructor

        public " + tn + @"() {
        }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
");

                        #endregion

                        #region Serial

                        sb.Append(@"
        #region Serial
        public byte[] GetBytes() {
            return new byte[][]
            {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
                this." + c.GetEscapeName() + @".GetBytes(),");
                        }
                        sb.Append(@"
            }.Combine();
        }
        public void Fill(byte[] buffer, ref int startIndex) {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
            this." + c.GetEscapeName() + @" = buffer." + c.DataType.GetToTypeMethod(c.Nullable) + @"(ref startIndex);");
                        }
                        sb.Append(@"
        }
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");

                }

                gr.Files.Add("DAL_Methods_Serial_Database_Views.cs", sb);
            }
            #endregion

            #region Sql Methods
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
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.Views." + sn + @"
{
");
                    foreach (var t in ts)
                    {
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
                if(count > 0)
                {
                    while(reader.Read())
                    {
                        var row = new " + tn + @"();
                        for(int i = 0; i < count; i++)
                        {");
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            ");
                            if (i > 0) sb.Append("else if(i < count && ");
                            else sb.Append("if(");
                            sb.Append(@"q.Columns[i] == @""" + c.Name.Replace("\"", "\"\"") + @""") {row." + cn + @" = ");
                            var s = "";
                            if (c.Nullable)
                            {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(i).Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(i))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(i)"));
                                sb.Append(@"reader.IsDBNull(i) ? null : " + s);
                            }
                            else
                            {
                                if (c.DataType.CheckIsBinaryType())
                                {
                                    sb.Append(@"reader.GetSqlBinary(i).Value");
                                }
                                else
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
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
                            " + cn + " = ");
                            var s = "";
                            if (c.Nullable)
                            {
                                s = c.DataType.CheckIsBinaryType() ? ("reader.GetSqlBinary(" + i + @").Value") : (c.DataType.CheckIsValueType() ? ("new " + c.DataType.GetNullableTypeName() + @"(reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @"))") : ("reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")"));
                                sb.Append(@"reader.IsDBNull(" + i + @") ? null : " + s);
                            }
                            else
                            {
                                if (c.DataType.CheckIsBinaryType())
                                {
                                    sb.Append(@"reader.GetSqlBinary(" + i + @").Value");
                                }
                                else
                                    sb.Append(@"reader." + c.DataType.GetDataReaderMethod() + @"(" + i + @")");
                            }
                            if (i < t.Columns.Count - 1) sb.Append(",");
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

                gr.Files.Add("DAL_Methods_Sql_Database_Views.cs", sb);
            }
            #endregion

            #endregion

            #region UserDefinedTableTypes

            #region Serial Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SqlLib;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.UserDefinedTableTypes." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @" : ISerial
    {");
                        #region Constructor

                        sb.Append(@"
        #region Constructor

        public " + tn + @"() {
        }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
");

                        #endregion

                        #region Serial

                        sb.Append(@"
        #region Serial
        public byte[] GetBytes() {
            return new byte[][]
            {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
                this." + c.GetEscapeName() + @".GetBytes(),");
                        }
                        sb.Append(@"
            }.Combine();
        }
        public void Fill(byte[] buffer, ref int startIndex) {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
            this." + c.GetEscapeName() + @" = buffer." + c.DataType.GetToTypeMethod(c.Nullable) + @"(ref startIndex);");
                        }
                        sb.Append(@"
        }
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Database_UserDefinedTableTypes.cs", sb);
            }
            #endregion

            #region Sql Extensions Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Data;
using System.Collections.Generic;
");
                var schemas = from tabletype in db.UserDefinedTableTypes group tabletype by tabletype.Schema;
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var tt in tts)
                    {
                        sb.Append(tt.Description.ToSummary(1));
                        sb.Append(@"
    public static partial class " + tt.GetEscapeName() + @"_Extensions
    {
        public static DataTable ToDataTable(this IEnumerable<" + tt.GetEscapeName() + @"> os)
        {
            var dt = new DataTable();");
                        for (int i = 0; i < tt.Columns.Count; i++)
                        {
                            sb.Append(@"
            dt.Columns.Add(""" + i + @""");");
                        }
                        sb.Append(@"
            foreach(var o in os) {
                var rowdata = new object[" + tt.Columns.Count + @"];");
                        for (int i = 0; i < tt.Columns.Count; i++)
                        {
                            var c = tt.Columns[i];
                            var cn = c.GetEscapeName();
                            if (c.Nullable) sb.Append(@"
                if(o." + cn + @" == null) rowdata[" + i + @"] = DBNull.Value;
                else rowdata[" + i + @"] = o." + cn + (c.DataType.CheckIsStringType() || c.DataType.CheckIsBinaryType()
                                ? @";" : @".Value;"));
                            else sb.Append(@"
                rowdata[" + i + @"] = o." + cn + @";");
                        }
                        sb.Append(@"
                dt.Rows.Add(rowdata);
            }
            return dt;
        }
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Sql_Ext_Database_UserDefinedTableTypes.cs", sb);
            }
            #endregion

            #endregion

            #region UserDefinedFunctions_Table

            #region Serial Methods
            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SqlLib;
");
                var schemas = from uf in db.UserDefinedFunctions where uf.FunctionType == MySmo.UserDefinedFunctionType.Table group uf by uf.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Database.UserDefinedFunctions." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @" : ISerial
    {");
                        #region Constructor

                        sb.Append(@"
        #region Constructor

        public " + tn + @"() {
        }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
");

                        #endregion

                        #region Serial

                        sb.Append(@"
        #region Serial
        public byte[] GetBytes() {
            return new byte[][]
            {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
                this." + c.GetEscapeName() + @".GetBytes(),");
                        }
                        sb.Append(@"
            }.Combine();
        }
        public void Fill(byte[] buffer, ref int startIndex) {");
                        foreach (var c in t.Columns)
                        {
                            sb.Append(@"
            this." + c.GetEscapeName() + @" = buffer." + c.DataType.GetToTypeMethod(c.Nullable) + @"(ref startIndex);");
                        }
                        sb.Append(@"
        }
        #endregion
");

                        #endregion

                        sb.Append(@"
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Database_UserDefinedFunctions_Table.cs", sb);
            }
            #endregion

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
                foreach (var sps in schemas)
                {

                    sb.Append(@"
namespace DAL.Database.StoredProcedures." + sps.Key.Escape() + @"
{
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
                                var pv = "";
                                string pdn;
                                if (p.DataType.SqlDataType == MySmo.SqlDataType.UserDefinedTableType)
                                {
                                    pdn = "List<UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + ">";
                                    pv = "UDTT." + p.DataType.Schema.Escape() + @"." + p.DataType.Name.Escape() + "_Extensions.ToDataTable(ps." + pn + @")";
                                }
                                else
                                {
                                    pdn = p.DataType.GetNullableTypeName().FillSpace(10);
                                    pv = "ps." + pn;
                                }

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
            if( ps.Exists_" + pn + @"() ) cmd.AddParameter(""" + pn + @""", " + pv + @", " + p.DataType.SqlDataType.GetSqlDbType(true) + @", " + (p.IsOutputParameter ? "true" : "false") + @");";
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
                        }
                        else
                        {
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

                gr.Files.Add("DAL_Methods_Sql_Database_StoredProcedures.cs", sb);
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
                foreach (var ts in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @" : LogicalNode<" + t.GetEscapeName() + @">
    {");
                        foreach (var c in t.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Expressions_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var v in vs)
                    {
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @" : LogicalNode<" + v.GetEscapeName() + @">
    {");
                        foreach (var c in v.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Expressions_Views.cs", sb);
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
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var tt in tts)
                    {
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @" : LogicalNode<" + tt.GetEscapeName() + @">
    {");
                        foreach (var c in tt.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Expressions_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var f in fs)
                    {
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @" : LogicalNode<" + f.GetEscapeName() + @">
    {");
                        foreach (var c in f.Columns)
                        {
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
                gr.Files.Add("DAL_Class_Expressions_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen Expressions Method

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Expressions;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Expressions_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var t in vs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Expressions_Views.cs", sb);
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
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var t in tts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Expressions_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Expressions.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var t in fs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Methods_Serial_Expressions_UserDefinedFunctions_Table.cs", sb);
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
                foreach (var ts in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        sb.Append(@"
    public partial class " + t.GetEscapeName() + @" : LogicalNode<" + t.GetEscapeName() + @">
    {");
                        foreach (var c in t.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Orientations_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var v in vs)
                    {
                        sb.Append(@"
    public partial class " + v.GetEscapeName() + @" : LogicalNode<" + v.GetEscapeName() + @">
    {");
                        foreach (var c in v.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Orientations_Views.cs", sb);
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
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var tt in tts)
                    {
                        sb.Append(@"
    public partial class " + tt.GetEscapeName() + @" : LogicalNode<" + tt.GetEscapeName() + @">
    {");
                        foreach (var c in tt.Columns)
                        {
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

                gr.Files.Add("DAL_Class_Orientation_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var f in fs)
                    {
                        sb.Append(@"
    public partial class " + f.GetEscapeName() + @" : LogicalNode<" + f.GetEscapeName() + @">
    {");
                        foreach (var c in f.Columns)
                        {
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
                gr.Files.Add("DAL_Class_Orientations_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen Orientations Method

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Orientations;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Orientations_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var t in vs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Orientations_Views.cs", sb);
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
                foreach (var tts in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var t in tts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Orientations_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {

                    sb.Append(@"
namespace DAL.Orientations.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var t in fs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Methods_Serial_Orientations_UserDefinedFunctions_Table.cs", sb);
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
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Tables." + sn + @"
{
");
                    foreach (var t in ts)
                    {
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

                gr.Files.Add("DAL_Class_Queries_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {
                    var sn = vs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var v in vs)
                    {
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

                gr.Files.Add("DAL_Class_Queries_Views.cs", sb);
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
                foreach (var tts in schemas)
                {
                    var sn = tts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var tt in tts)
                    {
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

                gr.Files.Add("DAL_Class_Queries_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {
                    var sn = fs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var f in fs)
                    {
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
                gr.Files.Add("DAL_Class_Queries_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen Queries Method

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.Queries;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    var sn = ts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Tables." + sn + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_QueriesTables.cs", sb);
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
                foreach (var vs in schemas)
                {
                    var sn = vs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var t in vs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Queries_Views.cs", sb);
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
                foreach (var tts in schemas)
                {
                    var sn = tts.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var t in tts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_Queries_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {
                    var sn = fs.Key.Escape();
                    sb.Append(@"
namespace DAL.Queries.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var t in fs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Methods_Serial_Queries_UserDefinedFunctions_Table.cs", sb);
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
                foreach (var ts in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    public partial class " + tn + @" : ColumnList<" + tn + @">
    {");
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + tn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            var c = t.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if (i < t.Columns.Count - 1) sb.Append(",");
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

                gr.Files.Add("DAL_Class_ColumnEnums_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var v in vs)
                    {
                        var vn = v.GetEscapeName();
                        sb.Append(@"
    public partial class " + vn + @" : ColumnList<" + vn + @">
    {");
                        for (int i = 0; i < v.Columns.Count; i++)
                        {
                            var c = v.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + vn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for (int i = 0; i < v.Columns.Count; i++)
                        {
                            var c = v.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if (i < v.Columns.Count - 1) sb.Append(",");
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

                gr.Files.Add("DAL_Class_ColumnEnums_Views.cs", sb);
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
                foreach (var tts in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var tt in tts)
                    {
                        var ttn = tt.GetEscapeName();
                        sb.Append(@"
    public partial class " + ttn + @" : ColumnList<" + ttn + @">
    {");
                        for (int i = 0; i < tt.Columns.Count; i++)
                        {
                            var c = tt.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + ttn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for (int i = 0; i < tt.Columns.Count; i++)
                        {
                            var c = tt.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if (i < tt.Columns.Count - 1) sb.Append(",");
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

                gr.Files.Add("DAL_Class_ColumnEnums_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var f in fs)
                    {
                        var fn = f.GetEscapeName();
                        sb.Append(@"
    public partial class " + fn + @" : ColumnList<" + fn + @">
    {");
                        for (int i = 0; i < f.Columns.Count; i++)
                        {
                            var c = f.Columns[i];
                            var cn = c.GetEscapeName();
                            sb.Append(@"
        public " + fn + @" " + cn + " { get { __columns.Add(" + i + "); return this; } }");
                        }
                        sb.Append(@"
        protected static string[] __cns = new string[]
        {");
                        for (int i = 0; i < f.Columns.Count; i++)
                        {
                            var c = f.Columns[i];
                            var cn = c.Name.Replace("\"", "\"\"");
                            sb.Append(@"
            @""" + cn + @"""");
                            if (i < f.Columns.Count - 1) sb.Append(",");
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
                gr.Files.Add("DAL_Class_ColumnEnums_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            #region Gen ColumnEnums Class Method

            #region Tables

            {
                sb.Clear();
                sb.Append(@"using System;
using System.Collections.Generic;
using SqlLib.ColumnEnums;
");
                var schemas = from table in db.Tables group table by table.Schema;
                foreach (var ts in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.Tables." + ts.Key.Escape() + @"
{
");
                    foreach (var t in ts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_ColumnEnums_Tables.cs", sb);
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
                foreach (var vs in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.Views." + vs.Key.Escape() + @"
{
");
                    foreach (var t in vs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_ColumnEnums_Views.cs", sb);
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
                foreach (var tts in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedTableTypes." + tts.Key.Escape() + @"
{
");
                    foreach (var t in tts)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }

                gr.Files.Add("DAL_Methods_Serial_ColumnEnums_UserDefinedTableTypes.cs", sb);
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
                foreach (var fs in schemas)
                {
                    sb.Append(@"
namespace DAL.ColumnEnums.UserDefinedFunctions." + fs.Key.Escape() + @"
{
");
                    foreach (var t in fs)
                    {
                        var tn = t.GetEscapeName();
                        sb.Append(@"
    partial class " + tn + @"
    {
        #region Serial

        public " + tn + @"() { }
        public " + tn + @"(byte[] buffer, ref int startIndex)
            : this() {
            Fill(buffer, ref startIndex);
        }
        public " + tn + @"(byte[] buffer)
            : this() {
            var startIndex = 0;
            Fill(buffer, ref startIndex);
        }

        #endregion
    }");
                    }
                    sb.Append(@"
}");
                }
                gr.Files.Add("DAL_Methods_Serial_ColumnEnums_UserDefinedFunctions_Table.cs", sb);
            }

            #endregion

            #endregion

            return gr;
        }

    }
}
