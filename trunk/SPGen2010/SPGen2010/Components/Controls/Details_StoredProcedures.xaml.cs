﻿using System;
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
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelpers.TryFindParent<DataGridRow>(source);
            if (row == null) return;
            e.Handled = true;

            var o = row.Item as StoredProcedure;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { o.Parent.Parent.Parent, o.Parent.Parent, o.Parent, o },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }

        private void _Up_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = this.StoredProcedures;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<NodeBase>(
                new NodeBase[] { o.Parent.Parent, o.Parent },
                (x, y) => x == y,
                item => (NodeBase)item
            );
        }
    }
}
