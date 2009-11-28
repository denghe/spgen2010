using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Fillers
{
    public static class ConnLogFiller
    {
        /// <summary>
        /// load connect log from disk
        /// </summary>
        public static DS.ConnLogDataTable Fill(this DS.ConnLogDataTable cl)
        {
            var fn = System.IO.Path.Combine(Environment.CurrentDirectory, "ConnLog.xml");   // same as Persister
            try
            {
                cl.ReadXml(fn);
            }
            catch { }
            return cl;
        }
    }
}
