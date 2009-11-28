using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using SPGen2010.Components.Modules;

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

        public readonly static DS.ConnLogDataTable ConnLog = null;
    }
}
