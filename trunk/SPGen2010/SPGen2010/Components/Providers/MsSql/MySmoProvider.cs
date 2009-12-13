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
        private Smo.Server _smo_server;
        public MySmoProvider(Smo.Server smo_server)
        {
            _smo_server = smo_server;
        }





        public void SetDataLimit(bool isIncludeExtendProperties)
        {
            if (isIncludeExtendProperties)
            {
                #region Set SMO SQL Struct Data Limit (with ExtendedProperties)

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
            else
            {
                #region Set SMO SQL Struct Data Limit

                _smo_server.SetDefaultInitFields(typeof(Smo.Database),
                    new String[] { "Name", "RecoveryModel", "CompatibilityLevel", "Collation", "Owner", "CreateDate" });

                _smo_server.SetDefaultInitFields(typeof(Smo.Schema),
                    new String[] { "Name", "IsSystemObject", "Owner" });

                _smo_server.SetDefaultInitFields(typeof(Smo.Table),
                    new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner" });

                _smo_server.SetDefaultInitFields(typeof(Smo.View),
                    new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner" });

                _smo_server.SetDefaultInitFields(typeof(Smo.StoredProcedure),
                    new String[] { "Name", "Schema", "IsSystemObject", "CreateDate", "Owner" });

                _smo_server.SetDefaultInitFields(typeof(Smo.UserDefinedFunction),
                    new String[] { "Name", "Schema", "FunctionType", "IsSystemObject", "CreateDate", "Owner" });

                if (_smo_server.VersionMajor >= 10)
                {
                    _smo_server.SetDefaultInitFields(typeof(Smo.UserDefinedTableType),
                        new String[] { "Name", "Schema", "CreateDate", "Owner" });
                }

                #endregion
            }
        }




        public MySmo.Server GetServer(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false)
        {
            var mysmo_server = new MySmo.Server();
            var mysmo_dbs = new List<MySmo.Database>();
            foreach (Smo.Database smo_db in _smo_server.Databases)
            {
                var mysmo_db = GetDatabase(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.ParentServer = mysmo_server;
                mysmo_dbs.Add(mysmo_db);
            }
            mysmo_server.Databases = mysmo_dbs;
            return mysmo_server;
        }

        public List<MySmo.Database> GetDatabases(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false)
        {
            var mysmo_dbs = new List<MySmo.Database>();
            foreach (Smo.Database smo_db in _smo_server.Databases)
                mysmo_dbs.Add(GetDatabase(smo_db, isIncludeExtendProperties, isIncludeChilds));
            return mysmo_dbs;
        }


        public List<MySmo.Schema> GetSchemas(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetSchemas(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public List<MySmo.Table> GetTables(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetTables(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public List<MySmo.View> GetViews(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetViews(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetUserDefinedFunctions(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetUserDefinedTableTypes(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public List<MySmo.StoredProcedure> GetStoredProcedures(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetStoredProcedures(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.Database GetDatabase(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetDatabase(_smo_server.Databases[database.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.Schema GetSchema(Oe.Schema schema, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetSchema(_smo_server.Databases[schema.Parent.Parent.Name].Schemas[schema.Name], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.Table GetTable(Oe.Table table, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetTable(_smo_server.Databases[table.Parent.Parent.Name].Tables[table.Name, table.Schema], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.View GetView(Oe.View view, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetView(_smo_server.Databases[view.Parent.Parent.Name].Views[view.Name, view.Schema], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction, bool isIncludeExtendProperties = true, bool isIncludeChilds = true) where T : Oe.UserDefinedFunctionBase
        {
            return GetUserDefinedFunction(_smo_server.Databases[userdefinedfunction.Parent.Parent.Name].UserDefinedFunctions[userdefinedfunction.Name, userdefinedfunction.Schema], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetUserDefinedTableType(_smo_server.Databases[userdefinedtabletype.Parent.Parent.Name].UserDefinedTableTypes[userdefinedtabletype.Name, userdefinedtabletype.Schema], isIncludeExtendProperties, isIncludeChilds);
        }

        public MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            return GetStoredProcedure(_smo_server.Databases[storedprocedure.Parent.Parent.Name].StoredProcedures[storedprocedure.Name, storedprocedure.Schema], isIncludeExtendProperties, isIncludeChilds);
        }




        #region Utils



        public MySmo.Database GetDatabase(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var mysmo_db = new MySmo.Database();

            mysmo_db.Name = smo_db.Name;
            mysmo_db.Owner = smo_db.Owner;
            mysmo_db.CreateTime = smo_db.CreateDate;

            if (isIncludeExtendProperties)
                mysmo_db.ExtendedProperties = NewExtendProperties(mysmo_db, smo_db.ExtendedProperties);

            if (isIncludeChilds)
            {
                mysmo_db.Schemas = GetSchemas(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.Tables = GetTables(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.Views = GetViews(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.UserDefinedFunctions = GetUserDefinedFunctions(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.UserDefinedTableTypes = GetUserDefinedTableTypes(smo_db, isIncludeExtendProperties, isIncludeChilds);
                mysmo_db.StoredProcedures = GetStoredProcedures(smo_db, isIncludeExtendProperties, isIncludeChilds);

                foreach (var o in mysmo_db.Schemas) o.ParentDatabase = mysmo_db;
                foreach (var o in mysmo_db.Tables) o.ParentDatabase = mysmo_db;
                foreach (var o in mysmo_db.Views) o.ParentDatabase = mysmo_db;
                foreach (var o in mysmo_db.UserDefinedFunctions) o.ParentDatabase = mysmo_db;
                foreach (var o in mysmo_db.UserDefinedTableTypes) o.ParentDatabase = mysmo_db;
                foreach (var o in mysmo_db.StoredProcedures) o.ParentDatabase = mysmo_db;
            }

            return mysmo_db;

            #endregion
        }

        public MySmo.Schema GetSchema(Smo.Schema smo_s, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_s = new MySmo.Schema();
            mysmo_s.ParentDatabase = null;
            mysmo_s.Name = smo_s.Name;
            mysmo_s.Owner = smo_s.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_s.ExtendedProperties = NewExtendProperties(mysmo_s, smo_s.ExtendedProperties);
            }
            return mysmo_s;

            #endregion
        }

        public MySmo.Table GetTable(Smo.Table smo_t, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement
            SetDataLimit(isIncludeExtendProperties);

            var mysmo_t = new MySmo.Table();
            mysmo_t.ParentDatabase = null;
            mysmo_t.Name = smo_t.Name;
            mysmo_t.Schema = smo_t.Schema;
            mysmo_t.CreateTime = smo_t.CreateDate;
            mysmo_t.Owner = smo_t.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_t.ExtendedProperties = NewExtendProperties(mysmo_t, smo_t.ExtendedProperties);
                if (mysmo_t.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_t.Description = mysmo_t.ExtendedProperties["MS_Description"];
            }
            if (isIncludeChilds)
            {
                mysmo_t.Columns = new List<MySmo.Column>();
                foreach (Smo.Column smo_c in smo_t.Columns)
                {
                    var mysmo_c = new MySmo.Column
                    {
                        ParentDatabase = null,
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
                    if (isIncludeExtendProperties)
                    {
                        mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                        if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                            mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                    }
                    mysmo_t.Columns.Add(mysmo_c);
                }
            }
            return mysmo_t;
            #endregion
        }

        public MySmo.View GetView(Smo.View smo_v, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_v = new MySmo.View();
            mysmo_v.ParentDatabase = null;
            mysmo_v.Name = smo_v.Name;
            mysmo_v.Schema = smo_v.Schema;
            mysmo_v.CreateTime = smo_v.CreateDate;
            mysmo_v.Owner = smo_v.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_v.ExtendedProperties = NewExtendProperties(mysmo_v, smo_v.ExtendedProperties);
                if (mysmo_v.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_v.Description = mysmo_v.ExtendedProperties["MS_Description"];
            }
            if (isIncludeChilds)
            {
                mysmo_v.Columns = new List<MySmo.Column>();
                foreach (Smo.Column smo_c in smo_v.Columns)
                {
                    var mysmo_c = new MySmo.Column
                    {
                        ParentDatabase = null,
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
                    // todo: 从 view 的扩展属性中取字段的备注
                    //if (isIncludeExtendProperties)
                    //{
                    //    mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                    //    if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                    //        mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                    //}
                    mysmo_v.Columns.Add(mysmo_c);
                }
            }
            return mysmo_v;

            #endregion
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction(Smo.UserDefinedFunction smo_f, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_f = new MySmo.UserDefinedFunction();

            mysmo_f.ParentDatabase = null;
            mysmo_f.Name = smo_f.Name;
            mysmo_f.Schema = smo_f.Schema;
            mysmo_f.CreateTime = smo_f.CreateDate;
            mysmo_f.Owner = smo_f.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_f.ExtendedProperties = NewExtendProperties(mysmo_f, smo_f.ExtendedProperties);
                if (mysmo_f.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_f.Description = mysmo_f.ExtendedProperties["MS_Description"];
            }
            if (isIncludeChilds)
            {
                mysmo_f.Parameters = new List<MySmo.Parameter>();
                foreach (Smo.UserDefinedFunctionParameter smo_p in smo_f.Parameters)
                {
                    var mysmo_p = new MySmo.Parameter
                    {
                        ParentDatabase = null,
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
                        //  //if (isIncludeExtendProperties)
                    };
                    // todo: 从 func 的扩展属性中取参数的备注
                    //if (isIncludeExtendProperties)
                    //{
                    //    mysmo_p.ExtendedProperties = NewExtendProperties(mysmo_p, smo_p.ExtendedProperties);
                    //    if (mysmo_p.ExtendedProperties.ContainsKey("MS_Description"))
                    //        mysmo_p.Description = mysmo_p.ExtendedProperties["MS_Description"];
                    //}
                    mysmo_f.Parameters.Add(mysmo_p);
                }
                if (smo_f.FunctionType == Smo.UserDefinedFunctionType.Table)
                {
                    mysmo_f.Columns = new List<MySmo.Column>();
                    foreach (Smo.Column smo_c in smo_f.Columns)
                    {
                        var mysmo_c = new MySmo.Column
                        {
                            ParentDatabase = null,
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
                        // todo: 从 func 的扩展属性中取字段的备注
                        //if (isIncludeExtendProperties)
                        //{
                        //    mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                        //    if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                        //        mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                        //}
                        mysmo_f.Columns.Add(mysmo_c);
                    }
                }
            }
            return mysmo_f;

            #endregion
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Smo.UserDefinedTableType smo_tt, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_tt = new MySmo.UserDefinedTableType();
            mysmo_tt.ParentDatabase = null;
            mysmo_tt.Name = smo_tt.Name;
            mysmo_tt.Schema = smo_tt.Schema;
            mysmo_tt.CreateTime = smo_tt.CreateDate;
            mysmo_tt.Owner = smo_tt.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_tt.ExtendedProperties = NewExtendProperties(mysmo_tt, smo_tt.ExtendedProperties);
                if (mysmo_tt.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_tt.Description = mysmo_tt.ExtendedProperties["MS_Description"];
            }
            if (isIncludeChilds)
            {
                mysmo_tt.Columns = new List<MySmo.Column>();
                foreach (Smo.Column smo_c in smo_tt.Columns)
                {
                    var mysmo_c = new MySmo.Column
                    {
                        ParentDatabase = null,
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
                    if (isIncludeExtendProperties)
                    {
                        mysmo_c.ExtendedProperties = NewExtendProperties(mysmo_c, smo_c.ExtendedProperties);
                        if (mysmo_c.ExtendedProperties.ContainsKey("MS_Description"))
                            mysmo_c.Description = mysmo_c.ExtendedProperties["MS_Description"];
                    }
                    mysmo_tt.Columns.Add(mysmo_c);
                }
            }
            return mysmo_tt;

            #endregion
        }

        public MySmo.StoredProcedure GetStoredProcedure(Smo.StoredProcedure smo_sp, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_sp = new MySmo.StoredProcedure();

            mysmo_sp.ParentDatabase = null;
            mysmo_sp.Name = smo_sp.Name;
            mysmo_sp.Schema = smo_sp.Schema;
            mysmo_sp.CreateTime = smo_sp.CreateDate;
            mysmo_sp.Owner = smo_sp.Owner;
            if (isIncludeExtendProperties)
            {
                mysmo_sp.ExtendedProperties = NewExtendProperties(mysmo_sp, smo_sp.ExtendedProperties);
                if (mysmo_sp.ExtendedProperties.ContainsKey("MS_Description"))
                    mysmo_sp.Description = mysmo_sp.ExtendedProperties["MS_Description"];
            }
            if (isIncludeChilds)
            {
                mysmo_sp.Parameters = new List<MySmo.Parameter>();
                foreach (Smo.StoredProcedureParameter smo_p in smo_sp.Parameters)
                {
                    var mysmo_p = new MySmo.Parameter
                    {
                        ParentDatabase = null,
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
                        //  //if (isIncludeExtendProperties)
                    };
                    //if (isIncludeExtendProperties)
                    //{
                    //    mysmo_p.ExtendedProperties = NewExtendProperties(mysmo_p, smo_p.ExtendedProperties);
                    //    if (mysmo_p.ExtendedProperties.ContainsKey("MS_Description"))
                    //        mysmo_p.Description = mysmo_p.ExtendedProperties["MS_Description"];
                    //}
                    mysmo_sp.Parameters.Add(mysmo_p);
                }
            }
            return mysmo_sp;

            #endregion
        }




        public List<MySmo.Schema> GetSchemas(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.Schema>();
            foreach (Smo.Schema item in smo_db.Schemas) os.Add(GetSchema(item));
            return os;

            #endregion
        }

        public List<MySmo.Table> GetTables(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.Table>();
            foreach (Smo.Table item in smo_db.Tables) os.Add(GetTable(item));
            return os;

            #endregion
        }

        public List<MySmo.View> GetViews(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.View>();
            foreach (Smo.View item in smo_db.Views) os.Add(GetView(item));
            return os;

            #endregion
        }

        public List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.UserDefinedFunction>();
            foreach (Smo.UserDefinedFunction item in smo_db.UserDefinedFunctions) os.Add(GetUserDefinedFunction(item));
            return os;

            #endregion
        }

        public List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.UserDefinedTableType>();
            foreach (Smo.UserDefinedTableType item in smo_db.UserDefinedTableTypes) os.Add(GetUserDefinedTableType(item));
            return os;

            #endregion
        }

        public List<MySmo.StoredProcedure> GetStoredProcedures(Smo.Database smo_db, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            var os = new List<MySmo.StoredProcedure>();
            foreach (Smo.StoredProcedure item in smo_db.StoredProcedures) os.Add(GetStoredProcedure(item));
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



        public static void FormatExtendProperties(MySmo.Table mysmo_t)
        {
            var eps = mysmo_t.ExtendedProperties;

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

            foreach (var key in delList) eps.Remove(key);       // 删除已合并键
            delList.Clear();

            foreach (var combine in combines) eps.Add(combine.Key, string.Join("", combine.Value));

            // 检查到如果当前 ep 为子项配置集（有可能子对象不支持多 ep 集合或不支持 ep）时，将 ep 应用到子项

            foreach (var o in eps)
            {
                if (o.Key == "ColumnSettings")
                {
                    var dt = new DS.ColumnExtendedInformationsDataTable();
                    dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(o.Value)));
                    foreach (var column in mysmo_t.Columns)
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
                    delList.Add(o.Key);
                }
                else if (o.Key == "ParameterSettings")
                {
                    var t = (MySmo.IParameterBase)parent;
                    var dt = new DS.ParameterExtendedInformationsDataTable();
                    dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(o.Value)));
                    foreach (var parameter in t.Parameters)
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
                    delList.Add(o.Key);
                }
                // else if   ResultSettings for SP
            }

            foreach (var key in delList) eps.Remove(key);       // 删除已颁布到子元素的并键
            delList.Clear();
        }

        #endregion

    }
}
