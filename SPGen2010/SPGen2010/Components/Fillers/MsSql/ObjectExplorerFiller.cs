using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using SPGen2010.Components.Modules;
using SmoUtils = SPGen2010.Components.Utils.MsSql.Utils;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;


namespace SPGen2010.Components.Fillers.MsSql
{
    public static partial class ObjectExplorerFiller
    {
        public static void Fill(this Oe.Database oedb, Database db)
        {
            
        }
    }
}
