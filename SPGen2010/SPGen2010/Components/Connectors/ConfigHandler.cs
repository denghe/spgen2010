using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;

using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Connectors
{
    public static class ConfigHandler
    {
        private static string _connLog_filename = System.IO.Path.Combine(Environment.CurrentDirectory, "ConnLog.xml");
        private static DS.ConnLogDataTable _connLog = null;
        private static object _connLog_sync = new object();

        /// <summary>
        /// return user's connect Log
        /// </summary>
        public static Func<DS.ConnLogDataTable> LoadConnLog = () =>
        {
            lock (_connLog_sync)
            {
                _connLog = new DS.ConnLogDataTable();
                try
                {
                    _connLog.ReadXml(_connLog_filename);
                }
                catch { }
                LoadConnLog = () => { return _connLog; };
                SaveConnLog = () => { _connLog.WriteXml(_connLog_filename); };
            }
            return _connLog;
        };

        /// <summary>
        /// save user's connect Log to disk
        /// </summary>
        public static Action SaveConnLog = () =>
        {
        };
    }
}
