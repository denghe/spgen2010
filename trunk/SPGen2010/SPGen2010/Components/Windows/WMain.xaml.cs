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
using System.IO;

using SPGen2010.Components.Connectors;
using SPGen2010.Components.Controls;
using SPGen2010.Components.Providers;
using SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Windows
{
    /// <summary>
    /// Interaction logic for WMain.xaml
    /// </summary>
    public partial class WMain : Window
    {
        #region Properties

        private static WMain _WMain = null;
        /// <summary>
        /// refresh to WMain instance
        /// </summary>
        public static WMain Instance
        {
            get { return _WMain; }
        }

        /// <summary>
        /// tips: set value after connected to database
        /// </summary>
        public IObjectExplorerProvider ObjectExplorerProvider { get; set; }

        /// <summary>
        /// tips: set value after connected to database
        /// </summary>
        public IMySmoProvider MySmoProvider { get; set; }

        /// <summary>
        /// tips: set value before connect to database
        /// </summary>
        public List<IGenerator> Generators { get; set; }

        #endregion

        #region load methods

        public WMain()
        {
            _WMain = this;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var gens = new List<IGenerator>();
            GeneratorLoader.InitComponents(ref gens);
            Generators = gens;

            PopupWindow_ConnectorBrowser();
        }

        #endregion

        #region menu click handlers

        private void _Quit_menuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region popup window methods

        private void PopupWindow_ConnectorBrowser()
        {
            this.ObjectExplorerProvider = null;
            _ObjectExplorer._TreeView.ItemsSource = null;
            _ObjectExplorer._TreeView.Items.Clear();

            var c = new WConnectorBrowser();
            c.ShowDialog();

            if (this.ObjectExplorerProvider != null)
            {
                _ObjectExplorer.BindData();
                _ObjectExplorer._TreeView.SetSelectedItem<NodeBase>(
                    new NodeBase[] { _ObjectExplorer.DataSource },
                    (x, y) => x == y,
                    item => (NodeBase)item
                );

                var o = _ObjectExplorer._TreeView.Items[0];
                var treeItem = _ObjectExplorer._TreeView.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
                if (treeItem != null || treeItem.HasItems) treeItem.IsExpanded = true;
            }
        }

        #endregion

    }
}
