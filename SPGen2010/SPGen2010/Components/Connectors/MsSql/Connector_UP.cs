using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Connectors.MsSql
{
    public class Connector_UP
    {
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
            var row = App.LoadConnLog().Where(o => o.InstanceType == "MsSql_UP").OrderBy(o=>o.CreateTime).Last();
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

        public Connector_UP()
        {
            Load();
        }
    }
}
