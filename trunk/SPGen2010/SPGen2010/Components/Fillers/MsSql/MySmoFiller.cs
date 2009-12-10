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

namespace SPGen2010.Components.Fillers.MsSql
{
    public partial class MySmoFiller : IMySmoFiller
    {
        private Smo.Server _smo_server;
        public MySmoFiller(Smo.Server smo_server)
        {
            _smo_server = smo_server;
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
            var mysmo_t = new MySmo.Table();
            var smo_db = _smo_server.Databases[table.Parent.Parent.Name];
            var smo_t = smo_db.Tables[table.Name, table.Schema];
            mysmo_t.ParentDatabase = null;
            mysmo_t.Name = smo_t.Name;
            mysmo_t.Schema = new MySmo.Schema { Name = table.Schema };
            if (isIncludeChilds)
            {
                mysmo_t.Columns = new List<MySmo.Column>(
                    from Smo.Column o in smo_t.Columns
                    select new MySmo.Column
                    {
                        ParentDatabase = null,
                        ParentTableBase = mysmo_t,
                        Name = o.Name,
                        DataType = new MySmo.DataType
                        {
                            Name = o.DataType.Name,
                            MaximumLength = o.DataType.MaximumLength,
                            NumericPrecision = o.DataType.NumericPrecision,
                            NumericScale = o.DataType.NumericScale,
                            SqlDataType = (MySmo.SqlDataType)(int)o.DataType.SqlDataType
                        },
                        Computed = o.Computed,
                        ComputedText = o.ComputedText,
                        Default = o.Default,
                        Identity = o.Identity,
                        IdentityIncrement = o.IdentityIncrement,
                        IdentitySeed = o.IdentitySeed,
                        InPrimaryKey = o.InPrimaryKey,
                        IsForeignKey = o.IsForeignKey,
                        Nullable = o.Nullable,
                        RowGuidCol = o.RowGuidCol
                    }
                );
            }
            if (isIncludeExtendProperties)
            {
                // todo
                //mysmo_t.ExtendedProperties
                //mysmo_t.Description = 
                //mysmo_t.Caption = 
                //mysmo_t.Summary = 
            }

            return mysmo_t;
        }

        public MySmo.View GetView(Oe.View view, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction, bool isIncludeExtendProperties = true, bool isIncludeChilds = true) where T : Oe.UserDefinedFunctionBase
        {
            throw new NotImplementedException();
        }

        public MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }

        public MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure, bool isIncludeExtendProperties = true, bool isIncludeChilds = true)
        {
            throw new NotImplementedException();
        }
    }
}
