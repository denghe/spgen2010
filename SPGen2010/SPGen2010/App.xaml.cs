using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;

using SPGen2010.Components.Modules;
using SPGen2010.Components.Fillers;
using SPGen2010.Components.Persisters;

namespace SPGen2010
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            SPGen2010.App app = new SPGen2010.App();
            app.InitializeComponent();
            app.Run();
        }
        private static string _connLog_filename = System.IO.Path.Combine(Environment.CurrentDirectory, "ConnLog.xml");
        private static DS.ConnLogDataTable _connLog = null;
        /// <summary>
        /// return user's connect Log
        /// </summary>
        public static Func<DS.ConnLogDataTable> LoadConnLog = () =>
        {
            App.LoadConnLog = () => { return App._connLog; };
            App.SaveConnLog = () => { App._connLog.WriteXml(App._connLog_filename); };
            App._connLog = new DS.ConnLogDataTable();
            try
            {
                App._connLog.ReadXml(_connLog_filename);
            }
            catch { }
            return App._connLog;
        };

        /// <summary>
        /// save user's connect Log to disk
        /// </summary>
        public static Action SaveConnLog = () =>
        {
        };

    }
}
