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
using SPGen2010.Components.Configures;
using SPGen2010.Components.Generators;
using SPGen2010.Components.Windows;
using SPGen2010.Components.Helpers.IO;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Configures_UserDefinedFunction_Table.xaml
    /// </summary>
    public partial class Configures_UserDefinedFunction_Table : UserControl
    {
        public Configures_UserDefinedFunction_Table()
        {
            InitializeComponent();
        }

        public Configures_UserDefinedFunction_Table(Oe.UserDefinedFunction_Table o)
            : this()
        {
            this.O = o;

            var cfgs = WMain.Instance.Configures.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.Table) > 0 && a.Validate(o);
            });

            foreach (var cfg in cfgs)
            {
                var c = new Label
                {
                    Content = (string)cfg.Properties[GenProperties.Caption]
                    ,
                    ToolTip = (string)cfg.Properties[GenProperties.Tips]
                    ,
                    Tag = cfg
                };
                c.MouseDown += new MouseButtonEventHandler(c_MouseDown);
                _Configures_StackPanel.Children.Add(c);
            }
        }

        void c_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Label;
            var cfg = c.Tag as IConfigure;
            cfg.Execute(this.O);
            
        }

        public Oe.UserDefinedFunction_Table O { get; set; }
    }
}
