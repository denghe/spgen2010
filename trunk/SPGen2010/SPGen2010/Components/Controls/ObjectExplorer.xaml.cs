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
            }
            #endregion

            #region Databases
            else if (typeof(Databases) == ot)
            {
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
            }
            #endregion

            #region Folder_Tables
            else if (typeof(Folder_Tables) == ot)
            {
            }
            #endregion

            #region Table
            else if (typeof(Oe.Table) == ot)
            {
            }
            #endregion

            #region Views
            else if (typeof(Folder_Views) == ot)
            {
            }
            #endregion

            #region View
            else if (typeof(View) == ot)
            {
            }
            #endregion

            #region UserDefinedFunctions
            else if (typeof(Folder_UserDefinedFunctions) == ot)
            {
            }
            #endregion

            #region UserDefinedFunction_Scale
            else if (typeof(UserDefinedFunction_Scale) == ot)
            {
            }
            #endregion

            #region UserDefinedFunction_Table
            else if (typeof(UserDefinedFunction_Table) == ot)
            {
            }
            #endregion

            #region UserDefinedTableTypes
            else if (typeof(Folder_UserDefinedTableTypes) == ot)
            {
            }
            #endregion

            #region UserDefinedTableType
            else if (typeof(UserDefinedTableType) == ot)
            {
            }
            #endregion

            #region StoredProcedures
            else if (typeof(Folder_StoredProcedures) == ot)
            {
            }
            #endregion

            #region StoredProcedure
            else if (typeof(StoredProcedure) == ot)
            {
            }
            #endregion

            #region Schemas
            else if (typeof(Folder_Schemas) == ot)
            {
            }
            #endregion

            #region Schema
            else if (typeof(Schema) == ot)
            {
            }
            #endregion

            // restore cursor
            Cursor = cc;
        }

    }
}
