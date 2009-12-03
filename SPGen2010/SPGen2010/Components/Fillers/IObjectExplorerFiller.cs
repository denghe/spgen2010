using System;
using SPGen2010.Components.Modules.ObjectExplorer;
namespace SPGen2010.Components.Fillers.MsSql
{
    public interface IObjectExplorerFiller
    {
        Database Fill(Database db, Action<double> progressNotify);
        Server Fill(Server server);
        string GetInstanceName();
    }
}
