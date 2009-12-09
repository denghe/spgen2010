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
    /// Interaction logic for Details_StoredProcedures.xaml
    /// </summary>
    public partial class Details_StoredProcedures : UserControl
    {
        public Details_StoredProcedures()
        {
            InitializeComponent();
        }

        public Details_StoredProcedures(Folder_StoredProcedures o)
            : this()
        {
            this.StoredProcedures = o;
            _Details_DataGrid.ItemsSource = o.StoredProcedures;
            _Path_Label.Content = o.Parent.Parent.Text + @"\" + o.Parent.Text + @"\StoredProcedures";
            _Count_Label.Content = o.StoredProcedures.Count.ToString();
        }

        public Folder_StoredProcedures StoredProcedures { get; set; }

        private void _Details_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
