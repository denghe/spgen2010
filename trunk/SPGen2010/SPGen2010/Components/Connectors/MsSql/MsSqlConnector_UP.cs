using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

namespace SPGen2010.Components.Connectors
{
    public class MsSqlConnector_UP
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
            //todo
        }

        /// <summary>
        /// save current connect information to log
        /// </summary>
        public void Save()
        {
            //todo
        }

        public MsSqlConnector_UP()
        {
            Load();
        }
    }
}
