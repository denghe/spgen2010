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
using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Windows;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_Tables.xaml
    /// </summary>
    public partial class Details_Tables : UserControl
    {
        public Details_Tables()
        {
            InitializeComponent();
        }

        public Details_Tables(Folder_Tables o)
            : this()
        {
            this.Tables = o;
            _Details_DataGrid.ItemsSource = o.Tables;
            _Path_Label.Content = o.Parent.Parent.Text + @"\" + o.Parent.Text + @"\Tables";
            _Count_Label.Content = o.Tables.Count.ToString();
        }

        public Folder_Tables Tables { get; set; }

        private void _Details_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelpers.TryFindParent<DataGridRow>(source);
            if (row == null) return;
            e.Handled = true;

            var o = row.Item as Oe.Table;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { o.Parent.Parent.Parent, o.Parent.Parent, o.Parent, o },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }
    }
}
