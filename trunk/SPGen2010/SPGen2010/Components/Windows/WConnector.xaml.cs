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

using SPGen2010.Components.Connectors;

namespace SPGen2010
{
    /// <summary>
    /// Interaction logic for WConnector.xaml
    /// </summary>
    public partial class WConnector : Window
    {
        public Server ServerInstance = null;
        private SqlConnector _connector = new SqlConnector();

        public WConnector()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _Message_Label.Content = "";

            _connector.Server = Properties.Settings.Default.Server;
            _connector.Username = Properties.Settings.Default.Username;
            _connector.Password = Properties.Settings.Default.Password;

            LayoutRoot.DataContext = _connector;
        }

        private void _Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;
            var errMsg = "";
            ServerInstance = _connector.Connect(ref errMsg);
            Cursor = cc;

            if (ServerInstance != null)
            {
                Properties.Settings.Default.Username = _connector.Username;
                Properties.Settings.Default.Password = _connector.Password;
                Properties.Settings.Default.Server = _connector.Server;
                Properties.Settings.Default.Save();

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
