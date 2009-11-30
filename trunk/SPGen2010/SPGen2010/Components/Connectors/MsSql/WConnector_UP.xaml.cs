using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SPGen2010.Components.Connectors.MsSql;
using SPGen2010.Components.Fillers.MsSql;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;

using Microsoft.SqlServer.Management.Smo;

namespace SPGen2010.Components.Connectors.MsSql
{
    /// <summary>
    /// Interaction logic for WConnector_UP.xaml
    /// </summary>
    public partial class WConnector_UP : Window
    {
        public Microsoft.SqlServer.Management.Smo.Server ServerInstance = null;
        private Connector_UP _connector = new Connector_UP();

        public WConnector_UP()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _Message_Label.Content = "";
            LayoutRoot.DataContext = _connector;
        }

        private void _Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;
            var errMsg = "";
            var ServerInstance = _connector.TryConnect(ref errMsg);
            Cursor = cc;

            if (ServerInstance != null)
            {
                _connector.Save();
                DialogResult = true;
                Close();

                var server = new Oe.Server { Text = _connector.Server };
                server.Databases = new Oe.Databases { Parent = server };
                server.Databases.Add(
                    new Oe.Database { Parent = server, Text = ServerInstance.Databases[0].Name }
                    .Fill(ServerInstance.Databases[0])
                );
                WMain.Instance._ObjectExplorer.Fill(server);
            }
            else _Message_Label.Content = errMsg;
        }

        private void _Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }


}
