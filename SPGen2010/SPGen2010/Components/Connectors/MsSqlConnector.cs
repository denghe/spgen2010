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
    public class MsSqlConnector
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
        /// 连接并返回服务器对象
        /// </summary>
        public Server Connect(ref string errMsg)
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
        /// 从用户配置文件中读入最后一次的登录信息（按CreateTime倒排取第一个）
        /// </summary>
        public void Load()
        {

        }
    }
}
