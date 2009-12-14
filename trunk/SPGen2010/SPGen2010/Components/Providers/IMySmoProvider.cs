using System;
using System.Collections.Generic;
using System.ComponentModel;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Providers
{
    public interface IMySmoProvider
    {
        List<MySmo.Database> GetDatabases(Oe.Server server);
        List<MySmo.Schema> GetSchemas(Oe.Database database);
        List<MySmo.Table> GetTables(Oe.Database database);
        List<MySmo.View> GetViews(Oe.Database database);
        List<MySmo.UserDefinedFunction> GetUserDefinedFunctions(Oe.Database database);
        List<MySmo.UserDefinedTableType> GetUserDefinedTableTypes(Oe.Database database);
        List<MySmo.StoredProcedure> GetStoredProcedures(Oe.Database database);

        MySmo.Server GetServer(Oe.Server server);
        MySmo.Database GetDatabase(Oe.Database database);
        MySmo.Schema GetSchema(Oe.Schema schema);
        MySmo.Table GetTable(Oe.Table table);
        MySmo.View GetView(Oe.View view);
        MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction) where T : Oe.UserDefinedFunctionBase;
        MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype);
        MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure);
    }
}
