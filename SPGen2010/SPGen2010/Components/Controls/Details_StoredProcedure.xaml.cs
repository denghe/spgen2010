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
    /// Interaction logic for Details_StoredProcedure.xaml
    /// </summary>
    public partial class Details_StoredProcedure : UserControl
    {
        public Details_StoredProcedure()
        {
            InitializeComponent();
        }

        public Details_StoredProcedure(StoredProcedure o)
            : this()
        {
            this.StoredProcedure = o;
            _Path_Label.Content = o.Text + @"\Databases";
        }

        public StoredProcedure StoredProcedure { get; set; }
    }
}
