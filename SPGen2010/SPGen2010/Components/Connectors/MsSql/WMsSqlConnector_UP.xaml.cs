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
using Microsoft.SqlServer.Management.Smo;

namespace SPGen2010.Components.Connectors.MsSql
{
    /// <summary>
    /// Interaction logic for WMsSqlConnector_UP.xaml
    /// </summary>
    public partial class WMsSqlConnector_UP : Window
    {
        public Server ServerInstance = null;
        private MsSqlConnector_UP _connector = new MsSqlConnector_UP();

        public WMsSqlConnector_UP()
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
            }
            else _Message_Label.Content = errMsg;
        }

        private void _Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }


}
