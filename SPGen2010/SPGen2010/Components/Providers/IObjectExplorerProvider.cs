﻿using System;
using SPGen2010.Components.Modules.ObjectExplorer;
using System.ComponentModel;
namespace SPGen2010.Components.Providers
{
    public interface IObjectExplorerProvider
    {
        Database Fill(Database db);
        Server Fill(Server server);
        string GetInstanceName();
    }
}
