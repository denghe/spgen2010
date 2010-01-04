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

namespace SPGen2010.Components.Windows
{
    /// <summary>
    /// Interaction logic for WOutputTexts.xaml
    /// </summary>
    public partial class WOutputTexts : Window
    {
        public WOutputTexts()
        {
            InitializeComponent();
        }

        public WOutputTexts(List<KeyValuePair<string, string>> texts)
            : this()
        {
            _Output_ListBox.ItemsSource = texts;
        }
    }
}
