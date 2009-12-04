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
    /// Interaction logic for Actions_UserDefinedFunctions.xaml
    /// </summary>
    public partial class Actions_UserDefinedFunctions : UserControl
    {
        public Actions_UserDefinedFunctions()
        {
            InitializeComponent();
        }

        public Actions_UserDefinedFunctions(Folder_UserDefinedFunctions o)
            : this()
        {
            this.UserDefinedFunctions = o;
            
        }

        public Folder_UserDefinedFunctions UserDefinedFunctions { get; set; }
    }
}
