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

using Oe = SPGen2010.Components.Modules.ObjectExplorer;
using MySmo = SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Windows;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_UserDefinedTableType.xaml
    /// </summary>
    public partial class Details_UserDefinedTableType : UserControl
    {
        public Details_UserDefinedTableType()
        {
            InitializeComponent();
        }

        public Details_UserDefinedTableType(Oe.UserDefinedTableType o)
            : this()
        {
            this.OeUserDefinedTableType = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\UserDefinedTableTypes\" + o.Text;

            var so = WMain.Instance.MySmoProvider.GetUserDefinedTableType(o);
            so.ParentDatabase = new MySmo.Database { Name = o.Parent.Parent.Name }; // for save
            this.MySmoUserDefinedTableType = so;
            this.DataContext = so;
        }

        public Oe.UserDefinedTableType OeUserDefinedTableType { get; set; }
        public MySmo.UserDefinedTableType MySmoUserDefinedTableType { get; set; }

        private void _Up_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = this.OeUserDefinedTableType;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<Oe.NodeBase>(
                new Oe.NodeBase[] { o.Parent.Parent.Parent, o.Parent.Parent, o.Parent },
                (x, y) => x == y,
                item => (Oe.NodeBase)item
            );
        }

        private void _Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor cc = Cursor;
            Cursor = Cursors.Wait;
            WMain.Instance.MySmoProvider.SaveExtendProperty(this.MySmoUserDefinedTableType);
            Cursor = cc;
        }
    }
}
