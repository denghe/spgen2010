using System;
using System.Collections.Generic;
using System.ComponentModel;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Providers
{
    public interface IMySmoProvider
    {
        MySmo.Server GetServer(Oe.Server oe_server);
        MySmo.Database GetDatabase(Oe.Database oe_database);
        MySmo.Schema GetSchema(Oe.Schema oe_schema);
        MySmo.Table GetTable(Oe.Table oe_table);
        MySmo.View GetView(Oe.View oe_view);
        MySmo.UserDefinedFunction GetUserDefinedFunction<T>(T oe_userdefinedfunction) where T : Oe.UserDefinedFunctionBase;
        MySmo.UserDefinedTableType GetUserDefinedTableType(Oe.UserDefinedTableType oe_userdefinedtabletype);
        MySmo.StoredProcedure GetStoredProcedure(Oe.StoredProcedure oe_storedprocedure);

        void SaveExtendProperty(MySmo.IExtendPropertiesBase mysmo_epb);
    }
}
