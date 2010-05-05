using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Generators;
using Smo = Microsoft.SqlServer.Management.Smo;
using SPGen2010.Components.Windows;
using System.Windows;

namespace SPGen2010.Components.Configures.MsSql.Database.DAL.SP
{
    class ResultFormatter : IConfigure
    {
        #region Settings

        public SqlElementTypes TargetSqlElementType
        {
            get { return SqlElementTypes.StoredProcedure; }
        }
        public Dictionary<GenProperties, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    this._properties = new Dictionary<GenProperties, object>();
                    this._properties.Add(GenProperties.Name, "C#/DAL/1/SP/ResultFormatter");
                    this._properties.Add(GenProperties.Caption, "设置返回结果类型(for DAL)");
                    this._properties.Add(GenProperties.Group, "C#");
                    this._properties.Add(GenProperties.Tips, "");
                }
                return this._properties;
            }
        }
        private Dictionary<GenProperties, object> _properties = null;

        #endregion

        #region Validate

        /// <summary>
        /// any sp
        /// </summary>
        public bool Validate(params Oe.NodeBase[] targetElements)
        {
            return true;
        }

        #endregion

        public void Execute(params Oe.NodeBase[] targetElements)
        {
            var smo_sp = (Smo.StoredProcedure)targetElements[0].Tag;
            var oe_sp = (Oe.StoredProcedure)targetElements[0];
            var mysmo_sp = WMain.Instance.MySmoProvider.GetStoredProcedure(oe_sp);

            var ss = smo_sp.Script();
            foreach (var s in ss)
                MessageBox.Show(s.ToString());
            // todo: send current sp & database to window
            var w = new WResultFormatter();
            w.ShowDialog();
        }

    }
}
