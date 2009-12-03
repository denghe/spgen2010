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

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;
using SPGen2010.Components.Modules;
using SPGen2010.Components.Windows;

namespace SPGen2010.Components.Connectors.MsSql
{
    /// <summary>
    /// Interaction logic for WConnector_UP.xaml
    /// </summary>
    public partial class WConnector_UP : Window
    {
        public Microsoft.SqlServer.Management.Smo.Server ServerInstance = null;

        public WConnector_UP()
        {
            InitializeComponent();

            Load();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _Message_Label.Content = "";
            LayoutRoot.DataContext = this;
        }

        private void _Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;
            var errMsg = "";
            var ServerInstance = TryConnect(ref errMsg);
            Cursor = cc;

            if (ServerInstance != null)
            {
                Save();
                DialogResult = true;
                Close();

                WMain.Instance._ObjectExplorer.Filler = new ObjectExplorerFiller { Server = ServerInstance };
                WMain.Instance._ObjectExplorer.BindData();
            }
            else _Message_Label.Content = errMsg;
        }

        private void _Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }








        private string _server = ".";
        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new Exception("Please type server's name or ip,port !");
                _server = value;
            }
        }

        private string _username = "";

        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new Exception("The username can't be empty !");
                _username = value;
            }
        }
        private string _password = "";

        public string Password
        {
            get { return _password; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new Exception("The password can't be empty !");
                _password = value;
            }
        }

        /// <summary>
        /// try to connect to server & return Smo.Server instance
        /// </summary>
        public Server TryConnect(ref string errMsg)
        {
            Server result = null;
            var sc = new ServerConnection();
            try
            {
                sc.ServerInstance = _server;
                sc.ConnectTimeout = 10;
                sc.LoginSecure = false;
                sc.Login = _username;
                sc.Password = _password;
                sc.Connect();
                result = new Server(sc);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// get latest mssql's connect log, fill to control
        /// </summary>
        public void Load()
        {
            DS.ConnLogRow row = null;
            try
            {
                row = App.LoadConnLog().Where(o => o.InstanceType == "MsSql_UP").OrderBy(o => o.CreateTime).Last();
            }
            catch { }
            if (row != null)
            {
                _username = row.Username;
                _password = row.Password;
                _server = row.InstanceName;
            }
        }

        /// <summary>
        /// save current connect information to log
        /// </summary>
        public void Save()
        {
            App.LoadConnLog().AddConnLogRow("MsSql_UP", _server, _username, _password, "", DateTime.Now);
            App.SaveConnLog();
        }

    }


}
