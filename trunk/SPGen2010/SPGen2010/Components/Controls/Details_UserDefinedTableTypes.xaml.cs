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
    /// Interaction logic for Details_UserDefinedTableTypes.xaml
    /// </summary>
    public partial class Details_UserDefinedTableTypes : UserControl
    {
        public Details_UserDefinedTableTypes()
        {
            InitializeComponent();
        }

        public Details_UserDefinedTableTypes(Folder_UserDefinedTableTypes o)
            : this()
        {
            this.UserDefinedTableTypes = o;
            _Path_Label.Content = o.Parent.Parent.Text + @"\" + o.Parent.Text + @"\UserDefinedFunctions";
        }

        public Folder_UserDefinedTableTypes UserDefinedTableTypes { get; set; }
    }
}
