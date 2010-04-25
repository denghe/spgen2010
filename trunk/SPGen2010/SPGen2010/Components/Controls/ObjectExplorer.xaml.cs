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

using SPGen2010.Components.Modules;
using SPGen2010.Components.Modules.ObjectExplorer;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Windows;
using SPGen2010.Components.Providers;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for ObjectExplorer.xaml
    /// </summary>
    public partial class ObjectExplorer : UserControl
    {

        public ObjectExplorer()
        {
            InitializeComponent();
        }

        public Server DataSource = null;

        public void BindData()
        {
            this.DataSource = new Server { Text = WMain.Instance.ObjectExplorerProvider.GetInstanceName() };
            WMain.Instance.ObjectExplorerProvider.Fill(this.DataSource);
            this._TreeView.ItemsSource = new Server[] { this.DataSource };
        }

        public void SetControl(UserControl details, UserControl actions, UserControl configures)
        {
            WMain.Instance._Details_Grid.Children.Clear();
            WMain.Instance._Details_Grid.Children.Add(details);

            WMain.Instance._Actions_Grid.Children.Clear();
            WMain.Instance._Actions_Grid.Children.Add(actions);

            WMain.Instance._Configures_Grid.Children.Clear();
            WMain.Instance._Configures_Grid.Children.Add(configures);
        }

        private void _TreeView_Selected(object sender, RoutedEventArgs e)
        {
            // backup cursor
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;

            // get current item
            var o = _TreeView.SelectedItem;

            #region Server
            if (o is Server)
            {
                var server = (Server)o;
                SetControl(new Details_Server(server), new Actions_Server(server));
            }
            #endregion

            #region Database
            else if (o is Database)
            {
                var db = (Database)o;
                if (db.Folders.Count == 0)
                {
                    try
                    {
                        WMain.Instance.ObjectExplorerProvider.Fill(db);
                    }
                    catch { }   // todo
                }
                SetControl(new Details_Database(db), new Actions_Database(db));
            }
            #endregion

            #region Folder_Tables
            else if (o is Folder_Tables)
            {
                var ts = (Folder_Tables)o;
                SetControl(new Details_Tables(ts), new Actions_Tables(ts));
            }
            #endregion

            #region Table
            else if (o is Oe.Table)
            {
                var t = (Oe.Table)o;
                SetControl(new Details_Table(t), new Actions_Table(t));
            }
            #endregion

            #region Views
            else if (o is Folder_Views)
            {
                var vs = (Folder_Views)o;
                SetControl(new Details_Views(vs), new Actions_Views(vs));
            }
            #endregion

            #region View
            else if (o is View)
            {
                var v = (View)o;
                SetControl(new Details_View(v), new Actions_View(v));
            }
            #endregion

            #region UserDefinedFunctions
            else if (o is Folder_UserDefinedFunctions)
            {
                var ufs = (Folder_UserDefinedFunctions)o;
                SetControl(new Details_UserDefinedFunctions(ufs), new Actions_UserDefinedFunctions(ufs));
            }
            #endregion

            #region UserDefinedFunction_Scale
            else if (o is UserDefinedFunction_Scale)
            {
                var suf = (UserDefinedFunction_Scale)o;
                SetControl(new Details_UserDefinedFunction_Scale(suf), new Actions_UserDefinedFunction_Scale(suf));
            }
            #endregion

            #region UserDefinedFunction_Table
            else if (o is UserDefinedFunction_Table)
            {
                var tuf = (UserDefinedFunction_Table)o;
                SetControl(new Details_UserDefinedFunction_Table(tuf), new Actions_UserDefinedFunction_Table(tuf));
            }
            #endregion

            #region UserDefinedTableTypes
            else if (o is Folder_UserDefinedTableTypes)
            {
                var tts = (Folder_UserDefinedTableTypes)o;
                SetControl(new Details_UserDefinedTableTypes(tts), new Actions_UserDefinedTableTypes(tts));
            }
            #endregion

            #region UserDefinedTableType
            else if (o is UserDefinedTableType)
            {
                var tt = (UserDefinedTableType)o;
                SetControl(new Details_UserDefinedTableType(tt), new Actions_UserDefinedTableType(tt));
            }
            #endregion

            #region StoredProcedures
            else if (o is Folder_StoredProcedures)
            {
                var sps = (Folder_StoredProcedures)o;
                SetControl(new Details_StoredProcedures(sps), new Actions_StoredProcedures(sps));
            }
            #endregion

            #region StoredProcedure
            else if (o is StoredProcedure)
            {
                var sp = (StoredProcedure)o;
                SetControl(new Details_StoredProcedure(sp), new Actions_StoredProcedure(sp));
            }
            #endregion

            #region Schemas
            else if (o is Folder_Schemas)
            {
                var ss = (Folder_Schemas)o;
                SetControl(new Details_Schemas(ss), new Actions_Schemas(ss));
            }
            #endregion

            #region Schema
            else if (o is Schema)
            {
                var s = (Schema)o;
                SetControl(new Details_Schema(s), new Actions_Schema(s));
            }
            #endregion

            // restore cursor
            Cursor = cc;
        }

    }
}
