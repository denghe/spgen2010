using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SPGen2010.Components.Modules;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Helpers.MsSql;

// SMO
using Smo = Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer;

namespace SPGen2010.Components.Providers.MsSql
{
    public partial class MySmoProvider : IMySmoProvider
    {
        public MySmo.Server GetServer(Oe.Server server)
        {
            #region implement

            var mysmo_server = new MySmo.Server();
            mysmo_server.Name = _smo_server.InstanceName;
            mysmo_server.Databases = new List<MySmo.Database>();
            foreach (Smo.Database smo_db in _smo_server.Databases) mysmo_server.Databases.Add(GetDatabase(smo_db, mysmo_server));
            return mysmo_server;

            #endregion
        }

        public MySmo.Database GetDatabase(Oe.Database database)
        {
            return GetDatabase(_smo_server.Databases[database.Name]);
        }

        public MySmo.Schema GetSchema(Oe.Schema schema)
        {
            return GetSchema(_smo_server.Databases[schema.Parent.Parent.Name].Schemas[schema.Name]);
        }

        public MySmo.Table GetTable(Oe.Table table)
        {
            return GetTable(_smo_server.Databases[table.Parent.Parent.Name].Tables[table.Name, table.Schema]);
        }

        public MySmo.View GetView(Oe.View view)
        {
            return GetView(_smo_server.Databases[view.Parent.Parent.Name].Views[view.Name, view.Schema]);
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction) where T : Oe.UserDefinedFunctionBase
        {
            return GetUserDefinedFunction(_smo_server.Databases[userdefinedfunction.Parent.Parent.Name].UserDefinedFunctions[userdefinedfunction.Name, userdefinedfunction.Schema], null);
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype)
        {
            return GetUserDefinedTableType(_smo_server.Databases[userdefinedtabletype.Parent.Parent.Name].UserDefinedTableTypes[userdefinedtabletype.Name, userdefinedtabletype.Schema]);
        }

        public MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure)
        {
            return GetStoredProcedure(_smo_server.Databases[storedprocedure.Parent.Parent.Name].StoredProcedures[storedprocedure.Name, storedprocedure.Schema]);
        }


        public void SaveExtendProperties(MySmo.Server epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.Database epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.Schema epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.Table epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.View epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.UserDefinedFunction epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.UserDefinedTableType epb)
        {
            throw new Exception("not implement");
        }
        public void SaveExtendProperties(MySmo.StoredProcedure epb)
        {
            throw new Exception("not implement");
        }



        #region Constructures

        public MySmoProvider(Smo.Server smo_server)
        {
            _smo_server = smo_server;
        }

        #endregion

        #region Utils

        private Smo.Server _smo_server;

        public void SetDataLimit()
        {
            #region Set SMO SQL Struct Data Limit

            _smo_server.SetDefaultInitFields(typeof(Smo.Database),
                new String[] { "Name", "RecoveryModel", "CompatibilityLevel", "Collation", "Owner", "CreateDate", "ExtendedProperties" });

            _smo_server.SetDefaultInitFields(typeof(Smo.Schema),
                new String[] { "Name", "IsSystemObject", "Owner", "ExtendedProperties" });

            _smo_server.SetDefaultInitFields(typeof(Smo.Table),
                new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner", "ExtendedProperties" });

            _smo_server.SetDefaultInitFields(typeof(Smo.View),
                new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner", "ExtendedProperties" });

            _smo_server.SetDefaultInitFields(typeof(Smo.StoredProcedure),
                new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner", "ExtendedProperties" });

            _smo_server.SetDefaultInitFields(typeof(Smo.UserDefinedFunction),
                new String[] { "Name", "Schema", "FunctionType", "IsSystemObject", "CreateDate", "Owner", "ExtendedProperties" });

            if (_smo_server.VersionMajor >= 10)
            {
                _smo_server.SetDefaultInitFields(typeof(Smo.UserDefinedTableType),
                    new String[] { "Name", "Schema", "CreateDate", "Owner", "ExtendedProperties" });
            }

            #endregion
        }

        public MySmo.Database GetDatabase(Smo.Database smo_db, MySmo.Server parent = null)
        {
            #region implement

            var mysmo_db = new MySmo.Database();

            mysmo_db.ParentServer = parent;
            mysmo_db.Name = smo_db.Name;
            mysmo_db.Owner = smo_db.Owner;
            mysmo_db.CreateTime = smo_db.CreateDate;
            mysmo_db.ExtendedProperties = NewExtendProperties(mysmo_db, smo_db.ExtendedProperties);
            mysmo_db.Schemas = GetSchemas(smo_db, mysmo_db);
            mysmo_db.Tables = GetTables(smo_db, mysmo_db);
            mysmo_db.Views = GetViews(smo_db, mysmo_db);
            mysmo_db.UserDefinedFunctions = GetUserDefinedFunctions(smo_db, mysmo_db);
            mysmo_db.UserDefinedTableTypes = GetUserDefinedTableTypes(smo_db, mysmo_db);
            mysmo_db.StoredProcedures = GetStoredProcedures(smo_db, mysmo_db);

            CombineExtendProperties(mysmo_db);

            return mysmo_db;

            #endregion
        }

        public MySmo.Schema GetSchema(Smo.Schema smo_s, MySmo.Database parent = null)
        {
            #region implement

            SetDataLimit();

            var mysmo_s = new MySmo.Schema();
            mysmo_s.ParentDatabase = parent;
            mysmo_s.Name = smo_s.Name;
            mysmo_s.Owner = smo_s.Owner;
            mysmo_s.ExtendedProperties = NewExtendProperties(mysmo_s, smo_s.ExtendedProperties);
            CombineExtendProperties(mysmo_s);

            return mysmo_s;

            #endregion
        }

        public MySmo.Table GetTable(Smo.Table smo_t, MySmo.Database parent = null)
        {
            #region implement
            SetDataLimit();

            var mysmo_t = new MySmo.Table();
            mysmo_t.ParentDatabase = parent;
            mysmo_t.Name = smo_t.Name;
            mysmo_t.Schema = smo_t.Schema;
            mysmo_t.CreateTime = smo_t.CreateDate;
            mysmo_t.Owner = smo_t.Owner;
            mysmo_t.ExtendedProperties = NewExtendProperties(mysmo_t, smo_t.ExtendedProperties);
            if (mysmo_t.ExtendedProperties.ContainsKey("MS_Description"))
                mysmo_t.Description = mysmo_t.ExtendedProperties["MS_Description"];
            mysmo_t.Columns = new List<MySmo.Column>();
            foreach (Smo.Column smo_c in smo_t.Columns)
            {
                var mysmo_c = new MySmo.Column
                {
                    ParentDatabase = parent,
                    ParentTableBase = mysmo_t,
                    Name = smo_c.Name,
                    DataType = new MySmo.DataType
                    {
                        Name = smo_c.DataType.Name,
                        MaximumLength = smo_c.DataType.MaximumLength,
                        NumericPrecision = smo_c.DataType.NumericPrecision,
                        NumericScale = smo_c.DataType.NumericScale,
                        SqlDataType = (MySmo.SqlDataType)(int)smo_c.DataType.SqlDataType
                    },
                    Computed = smo_c.Computed,
                    ComputedText = smo_c.ComputedText,
                    Default = smo_c.Default,
                    Identity = smo_c.Identity,
                    IdentityIncrement = smo_c.IdentityIncrement,
                    IdentitySeed = smo_c.IdentitySeed,
                    InPrimaryKey = smo_c.InPrimaryKey,
                    IsForeignKey = smo_c.IsForeignKey,
                    Nullable = smo_c.Nullable,
                    RowGuidCol = smo_c.RowGuidCol
                };
                mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                mysmo_t.Columns.Add(mysmo_c);
            }
            CombineExtendProperties(mysmo_t);

            return mysmo_t;
            #endregion
        }

        public MySmo.View GetView(Smo.View smo_v, MySmo.Database parent = null)
        {
            #region implement

            SetDataLimit();

            var mysmo_v = new MySmo.View();
            mysmo_v.ParentDatabase = parent;
            mysmo_v.Name = smo_v.Name;
            mysmo_v.Schema = smo_v.Schema;
            mysmo_v.CreateTime = smo_v.CreateDate;
            mysmo_v.Owner = smo_v.Owner;
            mysmo_v.ExtendedProperties = NewExtendProperties(mysmo_v, smo_v.ExtendedProperties);
            if (mysmo_v.ExtendedProperties.ContainsKey("MS_Description"))
                mysmo_v.Description = mysmo_v.ExtendedProperties["MS_Description"];
            mysmo_v.Columns = new List<MySmo.Column>();
            foreach (Smo.Column smo_c in smo_v.Columns)
            {
                var mysmo_c = new MySmo.Column
                {
                    ParentDatabase = parent,
                    ParentTableBase = mysmo_v,
                    Name = smo_c.Name,
                    DataType = new MySmo.DataType
                    {
                        Name = smo_c.DataType.Name,
                        MaximumLength = smo_c.DataType.MaximumLength,
                        NumericPrecision = smo_c.DataType.NumericPrecision,
                        NumericScale = smo_c.DataType.NumericScale,
                        SqlDataType = (MySmo.SqlDataType)(int)smo_c.DataType.SqlDataType
                    },
                    Computed = smo_c.Computed,
                    ComputedText = smo_c.ComputedText,
                    Default = smo_c.Default,
                    Identity = smo_c.Identity,
                    IdentityIncrement = smo_c.IdentityIncrement,
                    IdentitySeed = smo_c.IdentitySeed,
                    InPrimaryKey = smo_c.InPrimaryKey,
                    IsForeignKey = smo_c.IsForeignKey,
                    Nullable = smo_c.Nullable,
                    RowGuidCol = smo_c.RowGuidCol
                };
                mysmo_v.Columns.Add(mysmo_c);
            }
            CombineExtendProperties(mysmo_v);

            return mysmo_v;

            #endregion
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction(Smo.UserDefinedFunction smo_f, MySmo.Database parent = null)
        {
            #region implement

            SetDataLimit();

            var mysmo_f = new MySmo.UserDefinedFunction();

            mysmo_f.ParentDatabase = parent;
            mysmo_f.Name = smo_f.Name;
            mysmo_f.Schema = smo_f.Schema;
            mysmo_f.CreateTime = smo_f.CreateDate;
            mysmo_f.Owner = smo_f.Owner;
            mysmo_f.ExtendedProperties = NewExtendProperties(mysmo_f, smo_f.ExtendedProperties);
            if (mysmo_f.ExtendedProperties.ContainsKey("MS_Description"))
                mysmo_f.Description = mysmo_f.ExtendedProperties["MS_Description"];
            mysmo_f.Parameters = new List<MySmo.Parameter>();
            foreach (Smo.UserDefinedFunctionParameter smo_p in smo_f.Parameters)
            {
                var mysmo_p = new MySmo.Parameter
                {
                    ParentDatabase = parent,
                    ParentParameterBase = mysmo_f,
                    Name = smo_p.Name,
                    DefaultValue = smo_p.DefaultValue,
                    IsOutputParameter = false,
                    IsReadOnly = smo_p.IsReadOnly,
                    DataType = new MySmo.DataType
                    {
                        Name = smo_p.DataType.Name,
                        MaximumLength = smo_p.DataType.MaximumLength,
                        NumericPrecision = smo_p.DataType.NumericPrecision,
                        NumericScale = smo_p.DataType.NumericScale,
                        SqlDataType = (MySmo.SqlDataType)(int)smo_p.DataType.SqlDataType
                    }
                };
                mysmo_f.Parameters.Add(mysmo_p);
            }
            if (smo_f.FunctionType == Smo.UserDefinedFunctionType.Table)
            {
                mysmo_f.Columns = new List<MySmo.Column>();
                foreach (Smo.Column smo_c in smo_f.Columns)
                {
                    var mysmo_c = new MySmo.Column
                    {
                        ParentDatabase = parent,
                        ParentTableBase = mysmo_f,
                        Name = smo_c.Name,
                        DataType = new MySmo.DataType
                        {
                            Name = smo_c.DataType.Name,
                            MaximumLength = smo_c.DataType.MaximumLength,
                            NumericPrecision = smo_c.DataType.NumericPrecision,
                            NumericScale = smo_c.DataType.NumericScale,
                            SqlDataType = (MySmo.SqlDataType)(int)smo_c.DataType.SqlDataType
                        },
                        Computed = smo_c.Computed,
                        ComputedText = smo_c.ComputedText,
                        Default = smo_c.Default,
                        Identity = smo_c.Identity,
                        IdentityIncrement = smo_c.IdentityIncrement,
                        IdentitySeed = smo_c.IdentitySeed,
                        InPrimaryKey = smo_c.InPrimaryKey,
                        IsForeignKey = smo_c.IsForeignKey,
                        Nullable = smo_c.Nullable,
                        RowGuidCol = smo_c.RowGuidCol
                    };
                    mysmo_f.Columns.Add(mysmo_c);
                }
            }
            CombineExtendProperties(mysmo_f);

            return mysmo_f;

            #endregion
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Smo.UserDefinedTableType smo_tt, MySmo.Database parent = null)
        {
            #region implement

            SetDataLimit();

            var mysmo_tt = new MySmo.UserDefinedTableType();
            mysmo_tt.ParentDatabase = parent;
            mysmo_tt.Name = smo_tt.Name;
            mysmo_tt.Schema = smo_tt.Schema;
            mysmo_tt.CreateTime = smo_tt.CreateDate;
            mysmo_tt.Owner = smo_tt.Owner;
            mysmo_tt.ExtendedProperties = NewExtendProperties(mysmo_tt, smo_tt.ExtendedProperties);
            if (mysmo_tt.ExtendedProperties.ContainsKey("MS_Description"))
                mysmo_tt.Description = mysmo_tt.ExtendedProperties["MS_Description"];
            mysmo_tt.Columns = new List<MySmo.Column>();
            foreach (Smo.Column smo_c in smo_tt.Columns)
            {
                var mysmo_c = new MySmo.Column
                {
                    ParentDatabase = parent,
                    ParentTableBase = mysmo_tt,
                    Name = smo_c.Name,
                    DataType = new MySmo.DataType
                    {
                        Name = smo_c.DataType.Name,
                        MaximumLength = smo_c.DataType.MaximumLength,
                        NumericPrecision = smo_c.DataType.NumericPrecision,
                        NumericScale = smo_c.DataType.NumericScale,
                        SqlDataType = (MySmo.SqlDataType)(int)smo_c.DataType.SqlDataType
                    },
                    Computed = smo_c.Computed,
                    ComputedText = smo_c.ComputedText,
                    Default = smo_c.Default,
                    Identity = smo_c.Identity,
                    IdentityIncrement = smo_c.IdentityIncrement,
                    IdentitySeed = smo_c.IdentitySeed,
                    InPrimaryKey = smo_c.InPrimaryKey,
                    IsForeignKey = smo_c.IsForeignKey,
                    Nullable = smo_c.Nullable,
                    RowGuidCol = smo_c.RowGuidCol
                };
                mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                mysmo_tt.Columns.Add(mysmo_c);
            }
            CombineExtendProperties(mysmo_tt);

            return mysmo_tt;

            #endregion
        }

        public MySmo.StoredProcedure GetStoredProcedure(Smo.StoredProcedure smo_sp, MySmo.Database parent = null)
        {
            #region implement

            SetDataLimit();

            var mysmo_sp = new MySmo.StoredProcedure();

            mysmo_sp.ParentDatabase = parent;
            mysmo_sp.Name = smo_sp.Name;
            mysmo_sp.Schema = smo_sp.Schema;
            mysmo_sp.CreateTime = smo_sp.CreateDate;
            mysmo_sp.Owner = smo_sp.Owner;
            mysmo_sp.ExtendedProperties = NewExtendProperties(mysmo_sp, smo_sp.ExtendedProperties);
            if (mysmo_sp.ExtendedProperties.ContainsKey("MS_Description"))
                mysmo_sp.Description = mysmo_sp.ExtendedProperties["MS_Description"];
            mysmo_sp.Parameters = new List<MySmo.Parameter>();
            foreach (Smo.StoredProcedureParameter smo_p in smo_sp.Parameters)
            {
                var mysmo_p = new MySmo.Parameter
                {
                    ParentDatabase = parent,
                    ParentParameterBase = mysmo_sp,
                    Name = smo_p.Name,
                    DefaultValue = smo_p.DefaultValue,
                    IsOutputParameter = smo_p.IsOutputParameter,
                    IsReadOnly = smo_p.IsReadOnly,
                    DataType = new MySmo.DataType
                    {
                        Name = smo_p.DataType.Name,
                        MaximumLength = smo_p.DataType.MaximumLength,
                        NumericPrecision = smo_p.DataType.NumericPrecision,
                        NumericScale = smo_p.DataType.NumericScale,
                        SqlDataType = (MySmo.SqlDataType)(int)smo_p.DataType.SqlDataType
                    }
                };
                mysmo_sp.Parameters.Add(mysmo_p);
            }
            CombineExtendProperties(mysmo_sp);

            return mysmo_sp;

            #endregion
        }




        public List<MySmo.Schema> GetSchemas(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.Schema>();
            foreach (Smo.Schema item in smo_db.Schemas) os.Add(GetSchema(item, parent));
            return os;

            #endregion
        }

        public List<MySmo.Table> GetTables(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.Table>();
            foreach (Smo.Table item in smo_db.Tables) os.Add(GetTable(item, parent));
            return os;

            #endregion
        }

        public List<MySmo.View> GetViews(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.View>();
            foreach (Smo.View item in smo_db.Views) os.Add(GetView(item, parent));
            return os;

            #endregion
        }

        public List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.UserDefinedFunction>();
            foreach (Smo.UserDefinedFunction item in smo_db.UserDefinedFunctions) os.Add(GetUserDefinedFunction(item, parent));
            return os;

            #endregion
        }

        public List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.UserDefinedTableType>();
            foreach (Smo.UserDefinedTableType item in smo_db.UserDefinedTableTypes) os.Add(GetUserDefinedTableType(item, parent));
            return os;

            #endregion
        }

        public List<MySmo.StoredProcedure> GetStoredProcedures(Smo.Database smo_db, MySmo.Database parent = null)
        {
            #region implement

            var os = new List<MySmo.StoredProcedure>();
            foreach (Smo.StoredProcedure item in smo_db.StoredProcedures) os.Add(GetStoredProcedure(item, parent));
            return os;

            #endregion
        }



        public static MySmo.ExtendedProperties NewExtendProperties(MySmo.IExtendPropertiesBase parent, Smo.ExtendedPropertyCollection epc)
        {
            #region implement

            var eps = new MySmo.ExtendedProperties { ParentExtendPropertiesBase = parent };
            foreach (Smo.ExtendedProperty ep in epc) eps.Add(ep.Name, ep.Value as string);
            return eps;

            #endregion
        }



        public static void FormatExtendProperties(MySmo.Schema mysmo_s)
        {
            CombineExtendProperties(mysmo_s.ExtendedProperties);
        }
        public static void FormatExtendProperties(MySmo.Table mysmo_t)
        {
            CombineExtendProperties(mysmo_t.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_t);
        }
        public static void FormatExtendProperties(MySmo.View mysmo_v)
        {
            CombineExtendProperties(mysmo_v.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_v);
        }
        public static void FormatExtendProperties(MySmo.UserDefinedFunction mysmo_f)
        {
            CombineExtendProperties(mysmo_f.ExtendedProperties);
            if (mysmo_f.FunctionType == MySmo.UserDefinedFunctionType.Table)
                DistributeExtendProperties((MySmo.ITableBase)mysmo_f);
            DistributeExtendProperties((MySmo.IParameterBase)mysmo_f);
        }
        public static void FormatExtendProperties(MySmo.UserDefinedTableType mysmo_tt)
        {
            CombineExtendProperties(mysmo_tt.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_tt);
        }
        public static void FormatExtendProperties(MySmo.StoredProcedure mysmo_sp)
        {
            CombineExtendProperties(mysmo_sp.ExtendedProperties);
            DistributeExtendProperties((MySmo.IParameterBase)mysmo_sp);
        }



        public static void DistributeExtendProperties(MySmo.ITableBase mysmo_tb)
        {
            #region implement

            var epb = (MySmo.IExtendPropertiesBase)mysmo_tb;
            string s;
            if (!epb.ExtendedProperties.TryGetValue("ColumnSettings", out s)) return;
            var dt = new DS.ColumnExtendedInformationsDataTable();
            dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            foreach (var column in mysmo_tb.Columns)
            {
                var row = dt.FindByName(column.Name);
                if (row != null)
                {
                    foreach (System.Data.DataColumn dc in dt.Columns)
                    {
                        if (dc.Unique) continue;
                        var ceps = column.ExtendedProperties;
                        if (ceps.ContainsKey(dc.ColumnName)) continue;
                        ceps.Add(dc.ColumnName, (string)row[dc]);
                    }
                }
            }
            epb.ExtendedProperties.Remove("ColumnSettings");

            #endregion
        }


        public static void DistributeExtendProperties(MySmo.IParameterBase mysmo_pb)
        {
            #region implement

            var epb = (MySmo.IExtendPropertiesBase)mysmo_pb;
            string s;
            if (!epb.ExtendedProperties.TryGetValue("ParameterSettings", out s)) return;
            var dt = new DS.ParameterExtendedInformationsDataTable();
            dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            foreach (var parameter in mysmo_pb.Parameters)
            {
                var row = dt.FindByName(parameter.Name);
                if (row != null)
                {
                    foreach (System.Data.DataColumn dc in dt.Columns)
                    {
                        if (dc.Unique) continue;
                        parameter.ExtendedProperties.Add(dc.ColumnName, (string)row[dc]);
                    }
                }
            }
            epb.ExtendedProperties.Remove("ParameterSettings");

            #endregion
        }

        public static void CombineExtendProperties(MySmo.IExtendPropertiesBase epb)
        {
            CombineExtendProperties(epb.ExtendedProperties);
        }

        public static void CombineExtendProperties(MySmo.ExtendedProperties eps)
        {
            #region implement

            // combine part extended properties. template: ____Part_???_Of_???. sample: ____Part_001_Of_012
            var mark_part = "____Part_";

            var delList = new List<string>();
            var combines = new Dictionary<string, string[]>();
            foreach (var o in eps)
            {
                var key = o.Key;
                var len = key.Length;
                if (len > 19 && key.Substring(len - 19, 9) == mark_part)
                {
                    var purekey = key.Substring(0, len - 19);
                    var ss = key.Substring(len - 10).Split(new string[] { "_Of_" }, StringSplitOptions.None);
                    if (combines.ContainsKey(purekey))
                    {
                        var value = combines[purekey];
                        value[int.Parse(ss[0])] = o.Value;
                    }
                    else
                    {
                        var value = new string[int.Parse(ss[1])];
                        value[int.Parse(ss[0])] = o.Value;
                        combines.Add(key.Substring(0, len - 19), value);
                    }
                }
                delList.Add(key);
            }
            foreach (var key in delList) eps.Remove(key);
            foreach (var combine in combines) eps.Add(combine.Key, string.Join("", combine.Value));

            #endregion
        }

        #endregion

    }
}
