#region usings
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

//// SMO
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer;

#endregion

namespace SPGen2010
{
    /// <summary>
    /// Interaction logic for WMain.xaml
    /// </summary>
    public partial class WMain : Window
    {
        #region load methods

        public WMain()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (PopupWindow_Connector()) Refresh();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            MessageBox.Show("xx");
        }

        #endregion

        #region menu click handlers

        private void _Quit_menuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion


        #region popup window methods

        //private bool PopupWindow_Connector()
        //{
        //    var c = new WConnector();
        //    c.ShowDialog();
        //    if (c.DialogResult.HasValue && c.DialogResult.Value == true)
        //    {
                
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

    }
}
