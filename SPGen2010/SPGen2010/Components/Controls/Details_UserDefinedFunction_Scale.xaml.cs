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
    /// Interaction logic for Details_UserDefinedFunction_Scale.xaml
    /// </summary>
    public partial class Details_UserDefinedFunction_Scale : UserControl
    {
        public Details_UserDefinedFunction_Scale()
        {
            InitializeComponent();
        }

        public Details_UserDefinedFunction_Scale(Oe.UserDefinedFunction_Scale o)
            : this()
        {
            this.OeUserDefinedFunction_Scale = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\UserDefinedFunctions\" + o.Text;

            var so = WMain.Instance.MySmoProvider.GetUserDefinedFunction(o);
            so.ParentDatabase = new MySmo.Database { Name = o.Parent.Parent.Name }; // for save
            this.MySmoUserDefinedFunction = so;
            this.DataContext = so;
        }

        public Oe.UserDefinedFunction_Scale OeUserDefinedFunction_Scale { get; set; }
        public MySmo.UserDefinedFunction MySmoUserDefinedFunction { get; set; }

        private void _Up_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = this.OeUserDefinedFunction_Scale;
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
            WMain.Instance.MySmoProvider.SaveExtendProperty(this.MySmoUserDefinedFunction);
            Cursor = cc;
        }
    }
}
