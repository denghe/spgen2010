using System;
using System.Collections.Generic;
using System.ComponentModel;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Providers
{
    public interface IMySmoProvider
    {
        MySmo.Server GetServer(Oe.Server server);
        MySmo.Database GetDatabase(Oe.Database database);
        MySmo.Schema GetSchema(Oe.Schema schema);
        MySmo.Table GetTable(Oe.Table table);
        MySmo.View GetView(Oe.View view);
        MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T userdefinedfunction) where T : Oe.UserDefinedFunctionBase;
        MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType userdefinedtabletype);
        MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure storedprocedure);

        void SaveExtendProperty(MySmo.IExtendPropertiesBase epb);
    }
}
