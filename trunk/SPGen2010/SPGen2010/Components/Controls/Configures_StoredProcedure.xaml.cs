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
    /// Interaction logic for Configures_StoredProcedure.xaml
    /// </summary>
    public partial class Configures_StoredProcedure : UserControl
    {
        public Configures_StoredProcedure()
        {
            InitializeComponent();
        }

        public Configures_StoredProcedure(Oe.StoredProcedure o)
            : this()
        {
            this.O = o;

            var gens = WMain.Instance.Configures.FindAll(a =>
            {
                return (int)(a.TargetSqlElementType & SqlElementTypes.StoredProcedure) > 0 && a.Validate(o);
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
                _Configures_StackPanel.Children.Add(c);
            }
        }

        void c_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Label;
            var cfg = c.Tag as IConfigure;
            cfg.Execute(this.O);
            
        }

        public Oe.StoredProcedure O { get; set; }
    }
}
