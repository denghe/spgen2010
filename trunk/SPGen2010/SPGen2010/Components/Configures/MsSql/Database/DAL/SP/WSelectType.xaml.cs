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

        void WSelectType_Loaded(object sender = null, RoutedEventArgs e = null)
        {
            _SelectType_DbTable_RadioButton.Checked += new RoutedEventHandler(_SelectType_DbTable_RadioButton_Checked);
            _SelectType_Scalar_RadioButton.Checked += new RoutedEventHandler(_SelectType_Scalar_RadioButton_Checked);
            _SelectType_Table_RadioButton.Checked += new RoutedEventHandler(_SelectType_Table_RadioButton_Checked);
            _SelectType_View_RadioButton.Checked += new RoutedEventHandler(_SelectType_View_RadioButton_Checked);
            _SelectType_UserDefinedTableType_RadioButton.Checked += new RoutedEventHandler(_SelectType_UserDefinedTableType_RadioButton_Checked);
            _SelectType_Custom_RadioButton.Checked += new RoutedEventHandler(_SelectType_Custom_RadioButton_Checked);
            _New_Button.Click += new RoutedEventHandler(_Add_Button_Click);
            _Delete_Button.Click += new RoutedEventHandler(_Remove_Button_Click);
            _Submit_Button.Click += new RoutedEventHandler(_Submit_Button_Click);
        }

        private void DisableAll()
        {
            _SelectType_Scalar_DataType_ComboBox.IsEnabled =
            _SelectType_Scalar_CheckBox.IsEnabled =
            _SelectType_Table_ComboBox.IsEnabled =
            _SelectType_Table_CheckBox.IsEnabled =
            _SelectType_View_ComboBox.IsEnabled =
            _SelectType_View_CheckBox.IsEnabled =
            _SelectType_UserDefinedTableType_ComboBox.IsEnabled =
            _SelectType_UserDefinedTableType_CheckBox.IsEnabled =
            _SelectType_Custom_CheckBox.IsEnabled =
            _SelectType_Custom_DataGrid.IsEnabled =
            _New_Button.IsEnabled =
            _Delete_Button.IsEnabled = false;
        }

        private void _SelectType_DbTable_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
        }

        private void _SelectType_Scalar_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
            _SelectType_Scalar_DataType_ComboBox.IsEnabled =
            _SelectType_Scalar_CheckBox.IsEnabled = true;
        }

        private void _SelectType_Table_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
            _SelectType_Table_ComboBox.IsEnabled =
            _SelectType_Table_CheckBox.IsEnabled = true;
        }

        private void _SelectType_View_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
            _SelectType_View_ComboBox.IsEnabled =
            _SelectType_View_CheckBox.IsEnabled = true;
        }

        private void _SelectType_UserDefinedTableType_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
            _SelectType_UserDefinedTableType_ComboBox.IsEnabled =
            _SelectType_UserDefinedTableType_CheckBox.IsEnabled = true;
        }

        private void _SelectType_Custom_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            DisableAll();
            _SelectType_Custom_CheckBox.IsEnabled =
            _SelectType_Custom_DataGrid.IsEnabled =
            _New_Button.IsEnabled =
            _Delete_Button.IsEnabled = true;
        }

        private void _Add_Button_Click(object sender = null, RoutedEventArgs e = null)
        {

        }

        private void _Remove_Button_Click(object sender = null, RoutedEventArgs e = null)
        {

        }

        private void _Submit_Button_Click(object sender = null, RoutedEventArgs e = null)
        {

        }
    }
}
