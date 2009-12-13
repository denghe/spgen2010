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
    /// Interaction logic for Details_UserDefinedFunction_Table.xaml
    /// </summary>
    public partial class Details_UserDefinedFunction_Table : UserControl
    {
        public Details_UserDefinedFunction_Table()
        {
            InitializeComponent();
        }

        public Details_UserDefinedFunction_Table(Oe.UserDefinedFunction_Table o)
            : this()
        {
            this.UserDefinedFunction_Table = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\UserDefinedFunctions\" + o.Text;

            var so = WMain.Instance.MySmoProvider.GetUserDefinedFunction(o);
            this.MySmoUserDefinedFunction = so;
            this.DataContext = so;
        }

        public Oe.UserDefinedFunction_Table UserDefinedFunction_Table { get; set; }
        public MySmo.UserDefinedFunction MySmoUserDefinedFunction { get; set; }
    }
}
