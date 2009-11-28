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

using MsSql = SPGen2010.Components.Connectors.MsSql;

namespace SPGen2010.Components.Connectors
{
    /// <summary>
    /// Interaction logic for WConnectorBrowser.xaml
    /// </summary>
    public partial class WConnectorBrowser : Window
    {
        public WConnectorBrowser()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private void _MsSql_Instance_UP_Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            var w = new MsSql.WConnector_UP();
            w.ShowDialog();
            // ...
        }

        private void _MsSql_Instance_TC_Connect_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _MsSql_DBFile_Connect_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _SqLite_Connect_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Oracle_Connect_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _MySql_Connect_Button_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
