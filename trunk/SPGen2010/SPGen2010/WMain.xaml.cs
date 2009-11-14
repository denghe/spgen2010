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

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;
using System.IO;

#endregion

namespace SPGen2010
{
    /// <summary>
    /// Interaction logic for WMain.xaml
    /// </summary>
    public partial class WMain : Window
    {
        #region fields

        public Server ServerInstance = null;

        #endregion

        #region load methods

        public WMain()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (PopupWindow_Connector()) Refresh();
            try
            {
                var databases = from Database db in this.ServerInstance.Databases
                                where !db.IsSystemObject && db.IsAccessible
                                select db;
                foreach (var db in databases)
                {
                    _TreeView.Items.Add(new TreeViewWithIcons
                    {
                        HeaderText = db.Name,
                        Tag = db,
                        Icon = "SQL_Database.png".NewImageSource()
                    });
                }
                //_TreeView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region menu click handlers

        private void _Connect_menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PopupWindow_Connector()) Refresh();
        }

        private void _Quit_menuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region popup window methods

        private bool PopupWindow_Connector()
        {
            var c = new WConnector();
            c.ShowDialog();
            if (c.DialogResult.HasValue && c.DialogResult.Value == true)
            {
                ServerInstance = c.ServerInstance;
                return true;
            }
            return false;
        }

        #endregion

        #region refresh display

        private void Refresh()
        {

        }

        #endregion
    }
}
