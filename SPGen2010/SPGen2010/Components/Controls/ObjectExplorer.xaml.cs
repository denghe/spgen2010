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
                SetControl(new Details_Server(), new Actions_Server());
            }
            #endregion

            #region Databases
            else if (typeof(Databases) == ot)
            {
                //SetControl(new Details_Databases(), new Actions_Databases());
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
                //SetControl(new Details_Database(), new Actions_Database());
            }
            #endregion

            #region Folder_Tables
            else if (typeof(Folder_Tables) == ot)
            {
                //SetControl(new Details_Tables(), new Actions_Tables());
            }
            #endregion

            #region Table
            else if (typeof(Oe.Table) == ot)
            {
                //SetControl(new Details_Table(), new Actions_Table());
            }
            #endregion

            #region Views
            else if (typeof(Folder_Views) == ot)
            {
                //SetControl(new Details_Views(), new Actions_Views());
            }
            #endregion

            #region View
            else if (typeof(View) == ot)
            {
                //SetControl(new Details_View(), new Actions_View());
            }
            #endregion

            #region UserDefinedFunctions
            else if (typeof(Folder_UserDefinedFunctions) == ot)
            {
                //SetControl(new Details_UserDefinedFunctions(), new Actions_UserDefinedFunctions());
            }
            #endregion

            #region UserDefinedFunction_Scale
            else if (typeof(UserDefinedFunction_Scale) == ot)
            {
                //SetControl(new Details_UserDefinedFunction_Scale(), new Actions_UserDefinedFunction_Scale());
            }
            #endregion

            #region UserDefinedFunction_Table
            else if (typeof(UserDefinedFunction_Table) == ot)
            {
                //SetControl(new Details_UserDefinedFunction_Table(), new Actions_UserDefinedFunction_Table());
            }
            #endregion

            #region UserDefinedTableTypes
            else if (typeof(Folder_UserDefinedTableTypes) == ot)
            {
                //SetControl(new Details_UserDefinedTableTypes(), new Actions_UserDefinedTableTypes());
            }
            #endregion

            #region UserDefinedTableType
            else if (typeof(UserDefinedTableType) == ot)
            {
                //SetControl(new Details_UserDefinedTableType(), new Actions_UserDefinedTableType());
            }
            #endregion

            #region StoredProcedures
            else if (typeof(Folder_StoredProcedures) == ot)
            {
                //SetControl(new Details_StoredProcedures(), new Actions_StoredProcedures());
            }
            #endregion

            #region StoredProcedure
            else if (typeof(StoredProcedure) == ot)
            {
                //SetControl(new Details_StoredProcedure(), new Actions_StoredProcedure());
            }
            #endregion

            #region Schemas
            else if (typeof(Folder_Schemas) == ot)
            {
                //SetControl(new Details_Schemas(), new Actions_Schemas());
            }
            #endregion

            #region Schema
            else if (typeof(Schema) == ot)
            {
                //SetControl(new Details_Schema(), new Actions_Schema());
            }
            #endregion

            // restore cursor
            Cursor = cc;
        }

    }
}
