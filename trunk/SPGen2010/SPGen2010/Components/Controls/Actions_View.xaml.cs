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
    /// Interaction logic for Actions_View.xaml
    /// </summary>
    public partial class Actions_View : UserControl
    {
        public Actions_View()
        {
            InitializeComponent();
        }

        public Actions_View(View o)
            : this()
        {
            this.View = o;
            
        }

        public View View { get; set; }
    }
}
