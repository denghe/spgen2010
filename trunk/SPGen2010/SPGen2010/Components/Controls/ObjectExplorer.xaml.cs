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

using SPGen2010.Controls.ObjectExplorerModule;

namespace SPGen2010.Controls
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
            var folder_views = new Folder_Views(database1);

            this._TreeView.ItemsSource = new Server[] { server };
        }
    }
}