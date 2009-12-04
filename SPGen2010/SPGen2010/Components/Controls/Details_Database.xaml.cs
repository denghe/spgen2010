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
    /// Interaction logic for Details_Database.xaml
    /// </summary>
    public partial class Details_Database : UserControl
    {
        public Details_Database()
        {
            InitializeComponent();
        }

        public Details_Database(Database o)
            : this()
        {
            this.Database = o;
            _Path_Label.Content = o.Text + @"\Database";
        }

        public Database Database { get; set; }
    }
}
