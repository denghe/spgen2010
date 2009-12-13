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
    /// Interaction logic for Details_Schema.xaml
    /// </summary>
    public partial class Details_Schema : UserControl
    {
        public Details_Schema()
        {
            InitializeComponent();
        }

        public Details_Schema(Oe.Schema o)
            : this()
        {
            this.Schema = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\Schemas\" + o.Text;

            var so = WMain.Instance.MySmoProvider.GetSchema(o);
            this.MySmoSchema = so;
            this.DataContext = so;
        }

        public Oe.Schema Schema { get; set; }
        public MySmo.Schema MySmoSchema { get; set; }
    }
}
