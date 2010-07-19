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
using System.Windows.Shapes;

namespace SPGen2010.Components.Generators.MsSql.Database
{
    /// <summary>
    /// Interaction logic for WSettings.xaml
    /// </summary>
    public partial class WSettings : Window
    {
        public WSettings()
        {
            InitializeComponent();
        }

        private void _Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void _Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
