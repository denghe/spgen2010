using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Fillers
{
    public static class ConnLogFiller
    {
        public static DS.ConnLogDataTable Fill(this DS.ConnLogDataTable cl)
        {
            cl.ReadXml("");
            return cl;
        }
    }
}
