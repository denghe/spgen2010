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
using SPGen2010.Components.Windows;
using SPGen2010.Components.Configures;
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Configures_Server.xaml
    /// </summary>
    public partial class Configures_Server : UserControl
    {
        public Configures_Server()
        {
            InitializeComponent();
        }

        public Configures_Server(Oe.Server o)
            : this()
        {
            this.Server = o;

            var gens = WMain.Instance.Configures.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.Databases) > 0 && a.Validate(o);
            });

            foreach (var gen in gens)
            {
                _Configures_StackPanel.Children.Add(new Label
                {
                    Content = (string)gen.Properties[GenProperties.Caption]
                    ,
                    ToolTip = (string)gen.Properties[GenProperties.Tips]
                });
            }
        }

        public Oe.Server Server { get; set; }
    }
}
