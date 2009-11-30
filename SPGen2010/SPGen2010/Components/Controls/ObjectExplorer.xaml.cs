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

using SPGen2010.Components.Modules;
using SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Fillers.MsSql;

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

        public Server DataSource = null;
        public IObjectExplorerFiller Filler { get; set; }
        public void BindData()
        {
            this.DataSource = new Server { Text = this.Filler.GetInstanceName() };
            this.Filler.Fill(this.DataSource, true);
            this._TreeView.ItemsSource = new Server[] { this.DataSource };
        }


        private void _Server_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void _Database_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var c = (StackPanel)sender;
            var db = c.Tag as Database;
            //Filler.Fill(db);
            //MessageBox.Show(_TreeView.SelectedItem.ToString());
            //((TreeViewItem)c.Parent).ItemsSource = new Database[] { db };
            //this._TreeView.ItemsSource = new Server[] { this.DataSource };
        }

        private void _Tables_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _Views_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _UserDefinedFunctions_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _UserDefinedTableTypes_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _StoredProcedures_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _Schemas_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _Table_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _View_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _UserDefinedFunction_Scale_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _UserDefinedFunction_Table_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _UserDefinedTableType_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _StoredProcedure_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _Schema_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _TreeView_Selected(object sender, RoutedEventArgs e)
        {
            var o = _TreeView.SelectedItem;
            if (o == null) return;
            if (o.GetType() == typeof(Database))
            {
                var tvi = (TreeViewItem)e.Source;
                //tvi.SetBinding(
                //tvi.ItemsSource = new Database[] { (Database)_TreeView.SelectedItem };
            }
        }

    }
}