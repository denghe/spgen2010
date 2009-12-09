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
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Actions_UserDefinedFunction_Scale.xaml
    /// </summary>
    public partial class Actions_UserDefinedFunction_Scale : UserControl
    {
        public Actions_UserDefinedFunction_Scale()
        {
            InitializeComponent();
        }

        public Actions_UserDefinedFunction_Scale(Oe.UserDefinedFunction_Scale o)
            : this()
        {
            this.UserDefinedFunction_Scale = o;

            var gens = WMain.Instance.Generators.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.UserDefinedFunction_Scale) > 0 && a.Validate(o);
            });

            foreach (var gen in gens)
            {
                _Actions_StackPanel.Children.Add(new Label
                {
                    Content = (string)gen.Properties[GenProperties.Caption]
                    ,
                    ToolTip = (string)gen.Properties[GenProperties.Tips]
                });
            }
        }

        public Oe.UserDefinedFunction_Scale UserDefinedFunction_Scale { get; set; }
    }
}