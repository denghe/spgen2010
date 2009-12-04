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
using SPGen2010.Components.Fillers.MsSql;
using SPGen2010.Components.Windows;

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
        public IObjectExplorerFiller Filler { get; set; }

        public void BindData()
        {
            this.DataSource = new Server { Text = this.Filler.GetInstanceName() };
            this.Filler.Fill(this.DataSource);
            this._TreeView.ItemsSource = new Server[] { this.DataSource };
        }

        public void SetControl(UserControl details, UserControl actions)
        {
            WMain.Instance._Details_StackPanel.Children.Clear();
            WMain.Instance._Details_StackPanel.Children.Add(details);
            WMain.Instance._Actions_StackPanel.Children.Clear();
            WMain.Instance._Actions_StackPanel.Children.Add(actions);
        }

        private void _TreeView_Selected(object sender, RoutedEventArgs e)
        {
            // backup cursor
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;

            // get current item
            var o = _TreeView.SelectedItem;
            var ot = o.GetType();

            #region Server
            if (typeof(Server) == ot)
            {
                var server = (Server)o;
                SetControl(new Details_Server(server), new Actions_Server(server));
            }
            #endregion

            #region Databases
            else if (typeof(Databases) == ot)
            {
                //var dbs = (Databases)o;
                //SetControl(new Details_Databases(dbs), new Actions_Databases(dbs));
            }
            #endregion

            #region Database
            else if (typeof(Database) == ot)
            {
                var db = (Database)o;
                if (db.Folders.Count == 0)
                {
                    try
                    {
                        Filler.Fill(db);
                    }
                    catch { }   // todo
                }
                //SetControl(new Details_Database(db), new Actions_Database(db));
            }
            #endregion

            #region Folder_Tables
            else if (typeof(Folder_Tables) == ot)
            {
                //var ts = (Folder_Tables)o;
                //SetControl(new Details_Tables(ts), new Actions_Tables(ts));
            }
            #endregion

            #region Table
            else if (typeof(Oe.Table) == ot)
            {
                //var t = (Oe.Table)o;
                //SetControl(new Details_Table(t), new Actions_Table(t));
            }
            #endregion

            #region Views
            else if (typeof(Folder_Views) == ot)
            {
                //var vs = (Folder_Views)o;
                //SetControl(new Details_Views(vs), new Actions_Views(vs));
            }
            #endregion

            #region View
            else if (typeof(View) == ot)
            {
                //var v = (View)o;
                //SetControl(new Details_View(v), new Actions_View(v));
            }
            #endregion

            #region UserDefinedFunctions
            else if (typeof(Folder_UserDefinedFunctions) == ot)
            {
                //var ufs = (Folder_UserDefinedFunctions)o;
                //SetControl(new Details_UserDefinedFunctions(ufs), new Actions_UserDefinedFunctions(ufs));
            }
            #endregion

            #region UserDefinedFunction_Scale
            else if (typeof(UserDefinedFunction_Scale) == ot)
            {
                //var suf = (UserDefinedFunction_Scale)o;
                //SetControl(new Details_UserDefinedFunction_Scale(suf), new Actions_UserDefinedFunction_Scale(suf));
            }
            #endregion

            #region UserDefinedFunction_Table
            else if (typeof(UserDefinedFunction_Table) == ot)
            {
                //var tuf = (UserDefinedFunction_Table)o;
                //SetControl(new Details_UserDefinedFunction_Table(tuf), new Actions_UserDefinedFunction_Table(tuf));
            }
            #endregion

            #region UserDefinedTableTypes
            else if (typeof(Folder_UserDefinedTableTypes) == ot)
            {
                //var tts = (Folder_UserDefinedTableTypes)o;
                //SetControl(new Details_UserDefinedTableTypes(tts), new Actions_UserDefinedTableTypes(tts));
            }
            #endregion

            #region UserDefinedTableType
            else if (typeof(UserDefinedTableType) == ot)
            {
                //var tt(UserDefinedTableType)o;
                //SetControl(new Details_UserDefinedTableType(tt), new Actions_UserDefinedTableType(tt));
            }
            #endregion

            #region StoredProcedures
            else if (typeof(Folder_StoredProcedures) == ot)
            {
                //var sps(Folder_StoredProcedures)o;
                //SetControl(new Details_StoredProcedures(sps), new Actions_StoredProcedures(sps));
            }
            #endregion

            #region StoredProcedure
            else if (typeof(StoredProcedure) == ot)
            {
                //var sp(StoredProcedure)o;
                //SetControl(new Details_StoredProcedure(sp), new Actions_StoredProcedure(sp));
            }
            #endregion

            #region Schemas
            else if (typeof(Folder_Schemas) == ot)
            {
                //var ss(Folder_Schemas)o;
                //SetControl(new Details_Schemas(ss), new Actions_Schemas(ss));
            }
            #endregion

            #region Schema
            else if (typeof(Schema) == ot)
            {
                //var s(Schema)o;
                //SetControl(new Details_Schema(s), new Actions_Schema(s));
            }
            #endregion

            // restore cursor
            Cursor = cc;
        }

    }
}
