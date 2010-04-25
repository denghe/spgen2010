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
using MySmo = SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Generators;
using SPGen2010.Components.Windows;
using SPGen2010.Components.Helpers.IO;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Actions_Table.xaml
    /// </summary>
    public partial class Actions_Table : UserControl
    {
        public Actions_Table()
        {
            InitializeComponent();
        }

        public Actions_Table(Oe.Table o)
            : this()
        {
            this.O = o;

            var gens = WMain.Instance.Generators.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.Table) > 0 && a.Validate(o);
            });

            foreach (var gen in gens)
            {
                var c = new Label
                {
                    Content = (string)gen.Properties[GenProperties.Caption]
                    ,
                    ToolTip = (string)gen.Properties[GenProperties.Tips]
                    ,
                    Tag = gen
                };
                c.MouseDown += new MouseButtonEventHandler(c_MouseDown);
                _Actions_StackPanel.Children.Add(c);
            }
        }

        void c_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Label;
            var gen = c.Tag as IGenerator;
            var result = gen.Generate(this.O);
            OutputHelper.Output(result);
        }

        public Oe.Table O { get; set; }
    }
}
