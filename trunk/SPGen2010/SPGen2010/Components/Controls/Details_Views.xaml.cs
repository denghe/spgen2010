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
    /// Interaction logic for Details_Views.xaml
    /// </summary>
    public partial class Details_Views : UserControl
    {
        public Details_Views()
        {
            InitializeComponent();
        }

        public Details_Views(Folder_Views o)
            : this()
        {
            this.Views = o;
            _Path_Label.Content = o.Parent.Parent.Text + @"\" + o.Parent.Text + @"\Views";
            _Count_Label.Content = o.Views.Count.ToString();
        }

        public Folder_Views Views { get; set; }

        private void _Details_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
