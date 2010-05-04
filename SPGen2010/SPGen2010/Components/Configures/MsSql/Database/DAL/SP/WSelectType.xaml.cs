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

namespace SPGen2010.Components.Configures.MsSql.Database.DAL.SP
{
    /// <summary>
    /// Interaction logic for WSelectType.xaml
    /// </summary>
    public partial class WSelectType : Window
    {
        public WSelectType()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(WSelectType_Loaded);
        }

        void WSelectType_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void _SelectType_DbTable_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void _SelectType_Scalar_RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void _SelectType_Table_RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void _SelectType_View_RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void _SelectType_UserDefinedTableType_RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void _SelectType_Custom_RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void _Add_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Remove_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Submit_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
