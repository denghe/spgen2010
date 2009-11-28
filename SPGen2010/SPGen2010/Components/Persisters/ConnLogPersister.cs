using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Persisters
{
    public static class ConnLogPersister
    {
        public static void Persist(this DS.ConnLogDataTable cl)
        {
            cl.WriteXml("");
        }
    }
}
