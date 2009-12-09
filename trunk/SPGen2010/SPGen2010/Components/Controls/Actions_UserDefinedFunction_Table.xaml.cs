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
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Actions_UserDefinedFunction_Table.xaml
    /// </summary>
    public partial class Actions_UserDefinedFunction_Table : UserControl
    {
        public Actions_UserDefinedFunction_Table()
        {
            InitializeComponent();
        }

        public Actions_UserDefinedFunction_Table(Oe.UserDefinedFunction_Table o)
            : this()
        {
            this.UserDefinedFunction_Table = o;

            var gens = WMain.Instance.Generators.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.UserDefinedFunction_Table) > 0 && a.Validate(o);
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

        public Oe.UserDefinedFunction_Table UserDefinedFunction_Table { get; set; }
    }
}
