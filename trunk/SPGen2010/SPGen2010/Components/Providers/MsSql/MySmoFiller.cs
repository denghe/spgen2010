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
    public partial class MySmoFiller : IMySmoFiller
    {
        private Smo.Server _smo_server;
        public MySmoFiller(Smo.Server smo_server)
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
                    new String[] { "Name", "IsSystemObject", "Owner" });

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
                    new String[] { "Name", "RecoveryModel", "CompatibilityLevel", "Collation", "Owner", "CreateDate", "ExtendedProperties" });

                _smo_server.SetDefaultInitFields(typeof(Smo.Schema),
                    new String[] { "Name", "IsSystemObject", "Owner" });

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
        }





        public List<MySmo.Database> GetDatabases(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.Schema> GetSchemas(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.Table> GetTables(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.View> GetViews(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public List<MySmo.StoredProcedure> GetStoredProcedures(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public MySmo.Server GetServer(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false)
        {
            throw new NotImplementedException();
        }

        public MySmo.Database GetDatabase(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public MySmo.Schema GetSchema(Oe.Schema schema, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public MySmo.Table GetTable(Oe.Table table, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement
            SetDataLimit(isIncludeExtendProperties);

            var mysmo_t = new MySmo.Table();
            var smo_db = _smo_server.Databases[table.Parent.Parent.Name];
            var smo_t = smo_db.Tables[table.Name, table.Schema];
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

        public MySmo.View GetView(Oe.View view, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_v = new MySmo.View();
            var smo_db = _smo_server.Databases[view.Parent.Parent.Name];
            var smo_v = smo_db.Views[view.Name, view.Schema];
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

        public MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction, bool isIncludeExtendProperties = true, bool isIncludeChilds = true) where T : Oe.UserDefinedFunctionBase
        {
            throw new NotImplementedException();
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_tt = new MySmo.UserDefinedTableType();
            var smo_db = _smo_server.Databases[userdefinedtabletype.Parent.Parent.Name];
            var smo_tt = smo_db.UserDefinedTableTypes[userdefinedtabletype.Name, userdefinedtabletype.Schema];
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

        public MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            #region implement

            SetDataLimit(isIncludeExtendProperties);

            var mysmo_sp = new MySmo.StoredProcedure();
            var smo_db = _smo_server.Databases[storedprocedure.Parent.Parent.Name];
            var smo_sp = smo_db.StoredProcedures[storedprocedure.Name, storedprocedure.Schema];

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




        #region Utils

        public static MySmo.ExtendedProperties NewExtendProperties(MySmo.IExtendPropertiesBase parent, Smo.ExtendedPropertyCollection epc)
        {
            var eps = new MySmo.ExtendedProperties { ParentExtendPropertiesBase = parent };
            foreach (Smo.ExtendedProperty ep in epc) eps.Add(ep.Name, ep.Value as string);

            // 如果有找到别的项的命名＝当前项名 + "#@!Part_" + 数字　合并，纳入删除表
            var mark_part = "#@!Part_";

            var delList = new List<string>();
            foreach (var o in eps)
            {
                if (o.Key.Contains(mark_part)) continue;      // 分页项就不处理了　直接跳过
                var s = o.Value;
                // 根据　页码　部分　从小到大 排序取当前对象
                var parts = from ep in eps
                            where ep.Key.Contains(mark_part)
                            orderby int.Parse(ep.Key.Substring(ep.Key.LastIndexOf(mark_part) + 8))
                            select ep;
                foreach (var part in parts)
                {
                    s += part.Value;
                    delList.Add(part.Key);
                }
                if (s != o.Value) eps[o.Key] = s;
            }

            foreach (var key in delList) eps.Remove(key);       // 删除已合并键
            delList.Clear();

            // 检查到如果当前 ep 为子项配置集（有可能子对象不支持多 ep 集合或不支持 ep）时，将 ep 应用到子项

            foreach (var o in eps)
            {
                if (o.Key == "ColumnSettings")
                {
                    var t = (MySmo.ITableBase)parent;
                    var dt = new DS.ColumnExtendedInformationsDataTable();
                    dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(o.Value)));
                    foreach (var column in t.Columns)
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

            return eps;
        }

        #endregion

    }
}
