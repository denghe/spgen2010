using System;
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
    /// Interaction logic for Configures_Views.xaml
    /// </summary>
    public partial class Configures_Views : UserControl
    {
        public Configures_Views()
        {
            InitializeComponent();
        }

        public Configures_Views(Oe.Folder_Views o)
            : this()
        {
            this.Views = o;

            var gens = WMain.Instance.Configures.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.Views) > 0 && a.Validate(o);
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

        public Oe.Folder_Views Views { get; set; }
    }
}
