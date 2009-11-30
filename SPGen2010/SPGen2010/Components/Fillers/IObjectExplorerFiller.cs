using System;
using SPGen2010.Components.Modules.ObjectExplorer;
namespace SPGen2010.Components.Fillers.MsSql
{
    public interface IObjectExplorerFiller
    {
        Database Fill(Database db);
        Server Fill(Server server, bool is_fill_db_name_only);
        string GetInstanceName();
    }
}
