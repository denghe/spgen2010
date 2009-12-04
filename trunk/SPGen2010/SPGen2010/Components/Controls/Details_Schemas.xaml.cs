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
    /// Interaction logic for Details_Schemas.xaml
    /// </summary>
    public partial class Details_Schemas : UserControl
    {
        public Details_Schemas()
        {
            InitializeComponent();
        }

        public Details_Schemas(Schemas server)
            : this()
        {
            this.Schemas = server;
            _Path_Label.Content = server.Text + @"\Databases";
        }

        public Schemas Schemas { get; set; }
    }
}
