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
    /// Interaction logic for Actions_UserDefinedFunction_Table.xaml
    /// </summary>
    public partial class Actions_UserDefinedFunction_Table : UserControl
    {
        public Actions_UserDefinedFunction_Table()
        {
            InitializeComponent();
        }

        public Actions_UserDefinedFunction_Table(UserDefinedFunction_Table o)
            : this()
        {
            this.UserDefinedFunction_Table = o;
            
        }

        public UserDefinedFunction_Table UserDefinedFunction_Table { get; set; }
    }
}
