using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
using Smo = Microsoft.SqlServer.Management.Smo;
using Utils = SPGen2010.Components.Helpers.MsSql.Utils;
using SPGen2010.Components.Windows;
using SPGen2010.Components.Providers;

using SPGen2010.Components.Helpers.MySmo;

namespace SPGen2010.Components.Generators.MsSql.Table
{
    class DAL : IGenerator
    {
        #region Settings

        public SqlElementTypes TargetSqlElementType
        {
            get { return SqlElementTypes.Database; }
        }
        public Dictionary<GenProperties, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    this._properties = new Dictionary<GenProperties, object>();
                    this._properties.Add(GenProperties.Name, "DAL");
                    this._properties.Add(GenProperties.Caption, "根据 Database 生成 DAL 层");
                    this._properties.Add(GenProperties.Group, "C#");
                    this._properties.Add(GenProperties.Tips, "根据 Database 生成 DAL 层");
                }
                return this._properties;
            }
        }
        private Dictionary<GenProperties, object> _properties = null;

        #endregion

        #region Validate

        /// <summary>
        /// condations:
        /// only one primary key, and must be INTEGER (2bytes, 4bytes, 8bytes) type
        /// </summary>
        public bool Validate(params Oe.NodeBase[] targetElements)
        {
            return true;
        }

        #endregion

        public GenResult Generate(params Oe.NodeBase[] targetElements)
        {
            #region Init

            var gr = new GenResult(GenResultTypes.Files);
            var oe_db = (Oe.Database)targetElements[0];
            var db = WMain.Instance.MySmoProvider.GetDatabase(oe_db);

            #endregion

            #region Gen
            var sb = new StringBuilder();

            // 先试下扫表　生成类结构
            foreach (var t in db.Tables)
            {
                
            }


            #endregion

            //gr.Files.Add("2.txt", sb);
            return gr;
        }

    }
}
