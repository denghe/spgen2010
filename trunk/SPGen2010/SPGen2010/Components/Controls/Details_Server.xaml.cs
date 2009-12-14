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
    /// Interaction logic for Details_Server.xaml
    /// </summary>
    public partial class Details_Server : UserControl
    {
        public Details_Server()
        {
            InitializeComponent();
        }

        public Details_Server(Server o)
            : this()
        {
            this.Server = o;
            _Path_Label.Content = o.Text + @"\Databases";

            _Details_DataGrid.ItemsSource = o.Databases;
            _Count_Label.Content = o.Databases.Count.ToString();
        }

        public Server Server { get; set; }

        private void _Details_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelpers.TryFindParent<DataGridRow>(source);
            if (row == null) return;
            e.Handled = true;

            var db = row.Item as Database;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { db.Parent, db },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }

    }
}
