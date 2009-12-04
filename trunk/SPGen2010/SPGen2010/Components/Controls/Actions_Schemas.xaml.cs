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
    /// Interaction logic for Actions_Schemas.xaml
    /// </summary>
    public partial class Actions_Schemas : UserControl
    {
        public Actions_Schemas()
        {
            InitializeComponent();
        }

        public Actions_Schemas(Folder_Schemas server)
            : this()
        {
            this.Schemas = server;
            
        }

        public Folder_Schemas Schemas { get; set; }
    }
}
