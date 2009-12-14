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

using SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Windows;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_Database.xaml
    /// </summary>
    public partial class Details_Database : UserControl
    {
        public Details_Database()
        {
            InitializeComponent();
        }

        public Details_Database(Database o)
            : this()
        {
            this.Database = o;
            _Path_Label.Content = o.Parent.Text + @"\Databases\" + o.Text;

            _Details_DataGrid.ItemsSource = o.Folders;
        }

        public Database Database { get; set; }

        private void _Details_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelpers.TryFindParent<DataGridRow>(source);
            if (row == null) return;
            e.Handled = true;

            var o = row.Item as FolderBase;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { o.Parent.Parent, o.Parent, o },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }

        private void _Up_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = this.Database;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { o.Parent },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }
    }
}
