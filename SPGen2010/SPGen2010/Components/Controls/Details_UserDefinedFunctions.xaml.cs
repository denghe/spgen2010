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

using SPGen2010.Components.Modules.ObjectExplorer;

namespace SPGen2010.Components.Controls
{
    /// <summary>
    /// Interaction logic for Details_UserDefinedFunctions.xaml
    /// </summary>
    public partial class Details_UserDefinedFunctions : UserControl
    {
        public Details_UserDefinedFunctions()
        {
            InitializeComponent();
        }

        public Details_UserDefinedFunctions(UserDefinedFunctions o)
            : this()
        {
            this.UserDefinedFunctions = o;
            _Path_Label.Content = o.Parent.Parent.Parent.Text + @"\" + o.Parent.Parent.Text + @"\UserDefinedFunctions";
        }

        public UserDefinedFunctions UserDefinedFunctions { get; set; }
    }
}
