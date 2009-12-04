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
    /// Interaction logic for Actions_StoredProcedures.xaml
    /// </summary>
    public partial class Actions_StoredProcedures : UserControl
    {
        public Actions_StoredProcedures()
        {
            InitializeComponent();
        }

        public Actions_StoredProcedures(Folder_StoredProcedures o)
            : this()
        {
            this.StoredProcedures = o;
            
        }

        public Folder_StoredProcedures StoredProcedures { get; set; }
    }
}
