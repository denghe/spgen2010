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
    /// Interaction logic for Details_Table.xaml
    /// </summary>
    public partial class Details_Table : UserControl
    {
        public Details_Table()
        {
            InitializeComponent();
        }

        public Details_Table(Oe.Table o)
            : this()
        {
            this.OeTable = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\Tables\" + o.Text;

            var so = WMain.Instance.MySmoProvider.GetTable(o);
            this.MySmoTable = so;
            this.DataContext = so;
        }

        public Oe.Table OeTable { get; set; }
        public MySmo.Table MySmoTable { get; set; }

        private void _Up_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = this.OeTable;
            var tv = WMain.Instance._ObjectExplorer._TreeView;
            tv.SetSelectedItem<Oe.NodeBase>(
                new Oe.NodeBase[] { o.Parent.Parent.Parent, o.Parent.Parent, o.Parent },
                (x, y) => x == y,
                item => (Oe.NodeBase)item
            );
        }

        //private void TextBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var tb = sender as TextBox;
        //    tb.MinWidth = tb.Width = tb.MaxWidth = tb.ActualWidth;
        //}
    }
}