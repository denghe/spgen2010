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
using Microsoft.VisualC.StlClr;

namespace SPGen2010.Components.Windows
{
    /// <summary>
    /// Interaction logic for WOutputText.xaml
    /// </summary>
    public partial class WOutputText : Window
    {
        public WOutputText()
        {
            InitializeComponent();
        }
        public WOutputText(string text)
            : this()
        {
            this.Text = text;
            this.DataContext = this;
        }
        public WOutputText(GenericPair<string, string> code)
            : this(code.second)
        {
            this.Title = code.first;
        }

        public string Text { get; set; }
    }
}
