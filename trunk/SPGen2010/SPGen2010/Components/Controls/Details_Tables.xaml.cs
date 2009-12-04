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

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_Tables.xaml
    /// </summary>
    public partial class Details_Tables : UserControl
    {
        public Details_Tables()
        {
            InitializeComponent();
        }

        public Details_Tables(Tables o)
            : this()
        {
            this.Tables = o;
            _Path_Label.Content = o.Text + @"\Databases";
        }

        public Tables Tables { get; set; }
    }
}
