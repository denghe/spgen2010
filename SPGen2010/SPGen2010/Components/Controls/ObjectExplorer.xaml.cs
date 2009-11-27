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

            var server = new Server("xxx");
            var database1 = new Database(server, "db1");
            var database2 = new Database(server, "db2");
            var folder_tables1 = new Folder_Tables(database1);
            var folder_views1 = new Folder_Views(database1);
            var tables1_1 = new SPGen2010.Components.Modules.ObjectExplorer.Table(folder_tables1, "t1");
            var tables1_2 = new SPGen2010.Components.Modules.ObjectExplorer.Table(folder_tables1, "t2");
            var tables1_3 = new SPGen2010.Components.Modules.ObjectExplorer.Table(folder_tables1, "t3");
            var views1_1 = new View(folder_views1, "v1");
            var views1_2 = new View(folder_views1, "v2");

            this._TreeView.ItemsSource = new Server[] { server };
        }
    }
}
