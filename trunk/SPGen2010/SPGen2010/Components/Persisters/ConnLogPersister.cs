using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Persisters
{
    public static class ConnLogPersister
    {
        /// <summary>
        /// save connect log to disk
        /// </summary>
        public static DS.ConnLogDataTable Persist(this DS.ConnLogDataTable cl)
        {
            var fn = System.IO.Path.Combine(Environment.CurrentDirectory, "ConnLog.xml");   // same as Persister
            try
            {
                cl.WriteXml(fn);
            }
            catch { }
            return cl;
        }
    }
}
