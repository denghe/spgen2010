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


            if (typeof(Server) == ot)
            {
            }
            else if (typeof(Databases) == ot)
            {
            }
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
            else if (typeof(Folder_Tables) == ot)
            {
            }
            else if (typeof(Oe.Table) == ot)
            {
            }
            else if (typeof(Folder_Views) == ot)
            {
            }
            else if (typeof(View) == ot)
            {
            }
            else if (typeof(Folder_UserDefinedFunctions) == ot)
            {
            }
            else if (typeof(UserDefinedFunction_Scale) == ot)
            {
            }
            else if (typeof(UserDefinedFunction_Table) == ot)
            {
            }
            else if (typeof(Folder_UserDefinedTableTypes) == ot)
            {
            }
            else if (typeof(UserDefinedTableType) == ot)
            {
            }
            else if (typeof(Folder_StoredProcedures) == ot)
            {
            }
            else if (typeof(StoredProcedure) == ot)
            {
            }
            else if (typeof(Folder_Schemas) == ot)
            {
            }
            else if (typeof(Schema) == ot)
            {
            }

            // restore cursor
            Cursor = cc;
        }

    }
}
