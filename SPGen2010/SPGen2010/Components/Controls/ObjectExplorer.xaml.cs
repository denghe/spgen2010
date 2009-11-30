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
        }

        public Server[] DataSource = null;
        public void BindData(Server server)
        {
            this._TreeView.ItemsSource = DataSource = new Server[] { server };
        }
        public void BindData()
        {
            this._TreeView.ItemsSource = DataSource;
        }

    }
}
