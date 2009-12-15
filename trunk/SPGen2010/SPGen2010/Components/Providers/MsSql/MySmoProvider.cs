﻿using System;
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

        public MySmo.Database GetDatabase(Oe.Database oe_database)
        {
            return GetDatabase(_smo_server.Databases[oe_database.Name]);
        }

        public MySmo.Schema GetSchema(Oe.Schema oe_schema)
        {
            return GetSchema(_smo_server.Databases[oe_schema.Parent.Parent.Name].Schemas[oe_schema.Name]);
        }

        public MySmo.Table GetTable(Oe.Table oe_table)
        {
            return GetTable(_smo_server.Databases[oe_table.Parent.Parent.Name].Tables[oe_table.Name, oe_table.Schema]);
        }

        public MySmo.View GetView(Oe.View oe_view)
        {
            return GetView(_smo_server.Databases[oe_view.Parent.Parent.Name].Views[oe_view.Name, oe_view.Schema]);
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T oe_userdefinedfunction) where T : Oe.UserDefinedFunctionBase
        {
            return GetUserDefinedFunction(_smo_server.Databases[oe_userdefinedfunction.Parent.Parent.Name].UserDefinedFunctions[oe_userdefinedfunction.Name, oe_userdefinedfunction.Schema], null);
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType oe_userdefinedtabletype)
        {
            return GetUserDefinedTableType(_smo_server.Databases[oe_userdefinedtabletype.Parent.Parent.Name].UserDefinedTableTypes[oe_userdefinedtabletype.Name, oe_userdefinedtabletype.Schema]);
        }

        public MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure oe_storedprocedure)
        {
            return GetStoredProcedure(_smo_server.Databases[oe_storedprocedure.Parent.Parent.Name].StoredProcedures[oe_storedprocedure.Name, oe_storedprocedure.Schema]);
        }



        public void SaveExtendProperty(MySmo.IExtendPropertiesBase mysmo_epb, string key = null)
        {
            if (mysmo_epb is MySmo.Column)
            {
                var mysmo_c = mysmo_epb as MySmo.Column;
                if (mysmo_c.ParentTableBase is MySmo.Table)
                {
                    var mysmo_t = mysmo_c.ParentTableBase as MySmo.Table;
                    var smo_db = _smo_server.Databases[mysmo_t.ParentDatabase.Name];
                    var smo_t = smo_db.Tables[mysmo_t.Name, mysmo_t.Schema];
                    var smo_c = smo_t.Columns[mysmo_c.Name];
                    if (key == "MS_Description")
                    {
                        DeleteExtendProperty(smo_c, key);
                        SaveExtendProperty(smo_c, key, mysmo_c.ExtendedProperties[key]);
                    }
                    if (string.IsNullOrEmpty(key))
                        foreach (var mysmo_ep in mysmo_t.ExtendedProperties)
                        {
                            if (mysmo_ep.Key == "MS_Description")
                            {
                                DeleteExtendProperty(smo_c, mysmo_ep.Key);
                                SaveExtendProperty(smo_c, mysmo_ep.Key, mysmo_ep.Value);
                            }
                            else
                            {
                                var s = "";
                                if (mysmo_t.ExtendedProperties.TryGetValue("ColumnSettings", out s))
                                {
                                    mysmo_t.ExtendedProperties["ColumnSettings"] = AddKVPRowAndGetString(s, mysmo_ep.Key, mysmo_ep.Value);
                                }
                                else
                                {
                                    mysmo_t.ExtendedProperties.Add("ColumnSettings", AddKVPRowAndGetString(null, mysmo_ep.Key, mysmo_ep.Value));
                                }
                            }
                        }
                    else
                    {
                        var s = "";
                        if (mysmo_t.ExtendedProperties.TryGetValue("ColumnSettings", out s))
                        {
                            mysmo_t.ExtendedProperties["ColumnSettings"] = AddKVPRowAndGetString(s, key, mysmo_c.ExtendedProperties[key]);
                        }
                        else
                        {
                            mysmo_t.ExtendedProperties.Add("ColumnSettings", AddKVPRowAndGetString(null, key, mysmo_c.ExtendedProperties[key]));
                        }
                    }
                    SaveExtendProperty(mysmo_t, "ColumnSettings");
                }
                //else if ( View, Function, ...
            }
            else if (mysmo_epb is MySmo.Parameter)
            {

            }
            else if (mysmo_epb is MySmo.Table)
            {
                var mysmo_t = mysmo_epb as MySmo.Table;
                var smo_db = _smo_server.Databases[mysmo_t.ParentDatabase.Name];
                var smo_t = smo_db.Tables[mysmo_t.Name, mysmo_t.Schema];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_t.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_t, mysmo_ep.Key);
                        SaveExtendProperty(smo_t, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_t, key);
                    SaveExtendProperty(smo_t, key, mysmo_t.ExtendedProperties[key]);
                }
            }
            else if (mysmo_epb is MySmo.View)
            {
                var mysmo_v = mysmo_epb as MySmo.View;
                var smo_db = _smo_server.Databases[mysmo_v.ParentDatabase.Name];
                var smo_v = smo_db.Views[mysmo_v.Name, mysmo_v.Schema];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_v.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_v, mysmo_ep.Key);
                        SaveExtendProperty(smo_v, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_v, key);
                    SaveExtendProperty(smo_v, key, mysmo_v.ExtendedProperties[key]);
                }
            }
            else if (mysmo_epb is MySmo.UserDefinedFunction)
            {
                var mysmo_f = mysmo_epb as MySmo.UserDefinedFunction;
                var smo_db = _smo_server.Databases[mysmo_f.ParentDatabase.Name];
                var smo_f = smo_db.UserDefinedFunctions[mysmo_f.Name, mysmo_f.Schema];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_f.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_f, mysmo_ep.Key);
                        SaveExtendProperty(smo_f, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_f, key);
                    SaveExtendProperty(smo_f, key, mysmo_f.ExtendedProperties[key]);
                }
            }
            else if (mysmo_epb is MySmo.UserDefinedTableType)
            {
                var mysmo_tt = mysmo_epb as MySmo.UserDefinedTableType;
                var smo_db = _smo_server.Databases[mysmo_tt.ParentDatabase.Name];
                var smo_tt = smo_db.UserDefinedTableTypes[mysmo_tt.Name, mysmo_tt.Schema];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_tt.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_tt, mysmo_ep.Key);
                        SaveExtendProperty(smo_tt, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_tt, key);
                    SaveExtendProperty(smo_tt, key, mysmo_tt.ExtendedProperties[key]);
                }
            }
            else if (mysmo_epb is MySmo.StoredProcedure)
            {
                var mysmo_sp = mysmo_epb as MySmo.StoredProcedure;
                var smo_db = _smo_server.Databases[mysmo_sp.ParentDatabase.Name];
                var smo_sp = smo_db.StoredProcedures[mysmo_sp.Name, mysmo_sp.Schema];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_sp.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_sp, mysmo_ep.Key);
                        SaveExtendProperty(smo_sp, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_sp, key);
                    SaveExtendProperty(smo_sp, key, mysmo_sp.ExtendedProperties[key]);
                }
            }
            else if (mysmo_epb is MySmo.Schema)
            {
                var mysmo_s = mysmo_epb as MySmo.Schema;
                var smo_db = _smo_server.Databases[mysmo_s.ParentDatabase.Name];
                var smo_s = smo_db.Schemas[mysmo_s.Name];
                if (string.IsNullOrEmpty(key))
                    foreach (var mysmo_ep in mysmo_s.ExtendedProperties)
                    {
                        DeleteExtendProperty(smo_s, mysmo_ep.Key);
                        SaveExtendProperty(smo_s, mysmo_ep.Key, mysmo_ep.Value);
                    }
                else
                {
                    DeleteExtendProperty(smo_s, key);
                    SaveExtendProperty(smo_s, key, mysmo_s.ExtendedProperties[key]);
                }
            }
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
            mysmo_db.ExtendedProperties = GetExtendProperties(mysmo_db, smo_db.ExtendedProperties);
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
            mysmo_s.ExtendedProperties = GetExtendProperties(mysmo_s, smo_s.ExtendedProperties);
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
            mysmo_t.ExtendedProperties = GetExtendProperties(mysmo_t, smo_t.ExtendedProperties);
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
                mysmo_c.ExtendedProperties = GetExtendProperties(mysmo_c, smo_c.ExtendedProperties);
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
            mysmo_v.ExtendedProperties = GetExtendProperties(mysmo_v, smo_v.ExtendedProperties);
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
            mysmo_f.ExtendedProperties = GetExtendProperties(mysmo_f, smo_f.ExtendedProperties);
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
            mysmo_tt.ExtendedProperties = GetExtendProperties(mysmo_tt, smo_tt.ExtendedProperties);
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
                mysmo_c.ExtendedProperties = GetExtendProperties(mysmo_c, smo_c.ExtendedProperties);
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
            mysmo_sp.ExtendedProperties = GetExtendProperties(mysmo_sp, smo_sp.ExtendedProperties);
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

        public MySmo.ExtendedProperties GetExtendProperties(MySmo.IExtendPropertiesBase parent, Smo.ExtendedPropertyCollection epc)
        {
            #region implement

            var eps = new MySmo.ExtendedProperties { ParentExtendPropertiesBase = parent };
            foreach (Smo.ExtendedProperty ep in epc) eps.Add(ep.Name, ep.Value as string);
            return eps;

            #endregion
        }


        public void FormatExtendProperties(MySmo.Schema mysmo_s)
        {
            CombineExtendProperties(mysmo_s.ExtendedProperties);
        }
        public void FormatExtendProperties(MySmo.Table mysmo_t)
        {
            CombineExtendProperties(mysmo_t.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_t);
        }
        public void FormatExtendProperties(MySmo.View mysmo_v)
        {
            CombineExtendProperties(mysmo_v.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_v);
        }
        public void FormatExtendProperties(MySmo.UserDefinedFunction mysmo_f)
        {
            CombineExtendProperties(mysmo_f.ExtendedProperties);
            if (mysmo_f.FunctionType == MySmo.UserDefinedFunctionType.Table)
                DistributeExtendProperties((MySmo.ITableBase)mysmo_f);
            DistributeExtendProperties((MySmo.IParameterBase)mysmo_f);
        }
        public void FormatExtendProperties(MySmo.UserDefinedTableType mysmo_tt)
        {
            CombineExtendProperties(mysmo_tt.ExtendedProperties);
            DistributeExtendProperties((MySmo.ITableBase)mysmo_tt);
        }
        public void FormatExtendProperties(MySmo.StoredProcedure mysmo_sp)
        {
            CombineExtendProperties(mysmo_sp.ExtendedProperties);
            DistributeExtendProperties((MySmo.IParameterBase)mysmo_sp);
        }



        public void DistributeExtendProperties(MySmo.ITableBase mysmo_tb)
        {
            #region implement

            var epb = (MySmo.IExtendPropertiesBase)mysmo_tb;
            string s;
            if (!epb.ExtendedProperties.TryGetValue("ColumnSettings", out s)) return;
            var dt = new DS.KeyValuePairDataTable();
            dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            foreach (var column in mysmo_tb.Columns)
            {
                var row = dt.FindByKey(column.Name);
                if (row != null)
                {
                    var cdt = new DS.KeyValuePairDataTable();
                    cdt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(row.Value)));
                    foreach (var crow in cdt)
                        column.ExtendedProperties.Add(crow.Key, crow.Value);
                }
            }
            epb.ExtendedProperties.Remove("ColumnSettings");

            #endregion
        }

        public void DistributeExtendProperties(MySmo.IParameterBase mysmo_pb)
        {
            #region implement

            var epb = (MySmo.IExtendPropertiesBase)mysmo_pb;
            string s;
            if (!epb.ExtendedProperties.TryGetValue("ParameterSettings", out s)) return;
            var dt = new DS.KeyValuePairDataTable();
            dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));
            foreach (var parameter in mysmo_pb.Parameters)
            {
                var row = dt.FindByKey(parameter.Name);
                if (row != null)
                {
                    var cdt = new DS.KeyValuePairDataTable();
                    cdt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(row.Value)));
                    foreach (var crow in cdt)
                        parameter.ExtendedProperties.Add(crow.Key, crow.Value);
                }
            }
            epb.ExtendedProperties.Remove("ParameterSettings");

            #endregion
        }

        public void CombineExtendProperties(MySmo.IExtendPropertiesBase epb)
        {
            CombineExtendProperties(epb.ExtendedProperties);
        }

        public void CombineExtendProperties(MySmo.ExtendedProperties eps)
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


        /// <summary>
        /// save extended property to ep's ExtendedProperties
        /// </summary>
        /// <param name="ep">SMO object that has ExtendedProperties property.</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveExtendProperty(dynamic ep, string key, string value)
        {
            var length = value.Length;
            if (length > 3600)
            {
                var count = value.Length / 3600 + (value.Length % 3600 > 0 ? 1 : 0);
                for (int i = 0; i <= count; i++)
                {
                    var mod = value.Length % 3600;
                    var v = value.Substring(i * 3600, mod == 0 ? 3600 : mod);
                    SaveExtendProperty(ep, key + "____Part_" + i.ToString("###") + "_Of_" + count.ToString("###"), v);
                }
                return;
            }

            if (ep.ExtendedProperties.Contains(key))
            {
                if (string.IsNullOrEmpty(value))
                {
                    ep.ExtendedProperties[key].Drop();
                    ep.Alter();
                }
                else if (ep.ExtendedProperties[key].Value as string != value)
                {
                    ep.ExtendedProperties[key].Value = value;
                    ep.Alter();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ep.ExtendedProperties.Add(new Smo.ExtendedProperty((Smo.SqlSmoObject)ep, key, value));
                    ep.Alter();
                }
            }
        }


        /// <summary>
        /// delete extended property from ep's ExtendedProperties
        /// </summary>
        /// <param name="ep">SMO object that has ExtendedProperties property.</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void DeleteExtendProperty(dynamic ep, string key)
        {
            if (ep.ExtendedProperties.Contains(key))
            {
                ep.ExtendedProperties[key].Drop();
                ep.Alter();
            }
            else
            {
                var mark_part = "____Part_";
                foreach (Smo.ExtendedProperty p in ep.ExtendedProperties)
                {
                    var name = p.Name;
                    var len = key.Length;
                    if (len > 19 && name.Substring(len - 19, 9) == mark_part)
                    {
                        var ss = key.Substring(len - 10).Split(new string[] { "_Of_" }, StringSplitOptions.None);
                        var count = int.Parse(ss[1]);
                        for (int i = 0; i <= count; i++)
                        {
                            DeleteExtendProperty(ep, key + "____Part_" + i.ToString("###") + "_Of_" + count.ToString("###"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// add a key & value row to dt , and return dt xml
        /// </summary>
        public string AddKVPRowAndGetString(string xml, string key, string value)
        {
            var dt = new DS.KeyValuePairDataTable();
            if (!string.IsNullOrEmpty(xml)) dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
            dt.AddKeyValuePairRow(key, value);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            dt.AcceptChanges();
            dt.WriteXml(sw);
            return sb.ToString();
        }

        #endregion

    }
}