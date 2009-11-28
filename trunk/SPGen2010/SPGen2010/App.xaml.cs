using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using SPGen2010.Components.Modules;
using SPGen2010.Components.Fillers;

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
            // todo: init
            MessageBox.Show(App.Current.StartupUri.ToString());

            SPGen2010.App app = new SPGen2010.App();
            app.InitializeComponent();
            app.Run();
        }

        private static DS.ConnLogDataTable _connLogInstance = null;
        /// <summary>
        /// return user's connect Log (write into exe's dir)
        /// </summary>
        public static Func<DS.ConnLogDataTable> GetConnLogInstance = () =>
        {
            _connLogInstance = new DS.ConnLogDataTable().Fill();
            GetConnLogInstance = () => { return _connLogInstance; };
            return _connLogInstance;
        };

    }
}
