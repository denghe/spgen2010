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

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_Schemas.xaml
    /// </summary>
    public partial class Details_Schemas : UserControl
    {
        public Details_Schemas()
        {
            InitializeComponent();
        }

        public Details_Schemas(Folder_Schemas o)
            : this()
        {
            this.Schemas = o;
            _Path_Label.Content = o.Parent.Parent.Text + @"\" + o.Parent.Text + @"\Schemas";
            _Count_Label.Content = o.Schemas.Count.ToString();
        }

        public Folder_Schemas Schemas { get; set; }

        private void _Details_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
