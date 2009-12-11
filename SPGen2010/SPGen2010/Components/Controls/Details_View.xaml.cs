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
using SPGen2010.Components.Windows;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_View.xaml
    /// </summary>
    public partial class Details_View : UserControl
    {
        public Details_View()
        {
            InitializeComponent();
        }

        public Details_View(Oe.View o)
            : this()
        {
            this.OeView = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\Views\" + o.Text;

            var v = WMain.Instance.MySmoFiller.GetView(o);
            this.MySmoView = v;
            this.DataContext = v;
        }

        public Oe.View OeView { get; set; }
        public MySmo.View MySmoView { get; set; }
    }
}
