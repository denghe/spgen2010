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
    /// Interaction logic for Details_Server.xaml
    /// </summary>
    public partial class Details_Server : UserControl
    {
        public Details_Server()
        {
            InitializeComponent();
        }

        public Details_Server(Server o)
            : this()
        {
            this.Server = server;
            _Path_Label.Content = o.Text + @"\Databases";
        }

        public Server Server { get; set; }
    }
}
