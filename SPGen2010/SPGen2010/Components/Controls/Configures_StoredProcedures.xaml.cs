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
    /// Interaction logic for Configures_StoredProcedures.xaml
    /// </summary>
    public partial class Configures_StoredProcedures : UserControl
    {
        public Configures_StoredProcedures()
        {
            InitializeComponent();
        }

        public Configures_StoredProcedures(Oe.Folder_StoredProcedures o)
            : this()
        {
            this.StoredProcedures = o;

            var cfgs = WMain.Instance.Configures.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.StoredProcedures) > 0 && a.Validate(o);
            });

            foreach (var cfg in cfgs)
            {
                _Configures_StackPanel.Children.Add(new Label
                {
                    Content = (string)cfg.Properties[GenProperties.Caption]
                    ,
                    ToolTip = (string)cfg.Properties[GenProperties.Tips]
                });
            }
        }

        public Oe.Folder_StoredProcedures StoredProcedures { get; set; }
    }
}
