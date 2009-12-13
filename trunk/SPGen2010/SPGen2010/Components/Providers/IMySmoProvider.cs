using System;
using System.Collections.Generic;
using System.ComponentModel;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Providers
{
    public interface IMySmoProvider
    {
        List<MySmo.Database> GetDatabases(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false);
        List<MySmo.Schema> GetSchemas(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        List<MySmo.Table> GetTables(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        List<MySmo.View> GetViews(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        List<MySmo.StoredProcedure> GetStoredProcedures(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);

        MySmo.Server GetServer(Oe.Server server, bool isIncludeExtendProperties = true, bool isIncludeChilds = false);
        MySmo.Database GetDatabase(Oe.Database database, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        MySmo.Schema GetSchema(Oe.Schema schema, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        MySmo.Table GetTable(Oe.Table table, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        MySmo.View GetView(Oe.View view, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction, bool isIncludeExtendProperties = true, bool isIncludeChilds = true) where T : Oe.UserDefinedFunctionBase;
        MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
        MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure, bool isIncludeExtendProperties = true, bool isIncludeChilds = true);
    }
}
