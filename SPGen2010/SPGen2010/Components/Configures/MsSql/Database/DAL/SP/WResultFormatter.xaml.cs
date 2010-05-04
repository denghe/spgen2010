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
    /// Interaction logic for WResultFormatter.xaml
    /// </summary>
    public partial class WResultFormatter : Window
    {
        public WResultFormatter()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(WResultFormatter_Loaded);

            _ResultType_DbSet_RadioButton.Checked += new RoutedEventHandler(_ResultType_DbSet_RadioButton_Checked);
            _ResultType_Custom_RadioButton.Checked += new RoutedEventHandler(_ResultType_Custom_RadioButton_Checked);
            _SelectType_None_RadioButton.Checked += new RoutedEventHandler(_SelectType_None_RadioButton_Checked);
            _SelectType_Scalar_RadioButton.Checked += new RoutedEventHandler(_SelectType_Scalar_RadioButton_Checked);
            _SelectType_Custom_RadioButton.Checked += new RoutedEventHandler(_SelectType_Custom_RadioButton_Checked);
        }

        void WResultFormatter_Loaded(object sender, RoutedEventArgs e)
        {
            // todo: restore current settings

            _ResultType_DbSet_RadioButton_Checked();
            _SelectType_None_RadioButton_Checked();
            _SelectType_Scalar_DataType_ComboBox.SelectedIndex = 0;
        }

        private void _ResultType_DbSet_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            _ResultType_Custom_GroupBox.IsEnabled = false;
        }

        private void _ResultType_Custom_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            _ResultType_Custom_GroupBox.IsEnabled = true;
        }

        private void _SelectType_None_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            _SelectType_Custom_DataGrid.IsEnabled =
                _New_Button.IsEnabled =
                _Edit_Button.IsEnabled =
                _Delete_Button.IsEnabled =
                _SelectType_Scalar_StackPanel.IsEnabled = false;
        }

        private void _SelectType_Scalar_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            _SelectType_Custom_DataGrid.IsEnabled =
                _New_Button.IsEnabled =
                _Edit_Button.IsEnabled =
                _Delete_Button.IsEnabled = false;
            _SelectType_Scalar_StackPanel.IsEnabled = true;
        }

        private void _SelectType_Custom_RadioButton_Checked(object sender = null, RoutedEventArgs e = null)
        {
            _SelectType_Custom_DataGrid.IsEnabled =
                _New_Button.IsEnabled =
                _Edit_Button.IsEnabled =
                _Delete_Button.IsEnabled = true;
            _SelectType_Scalar_StackPanel.IsEnabled = false;
        }

        private void _New_Button_Click(object sender, RoutedEventArgs e)
        {
            var w = new WSelectType();
            w.ShowDialog();
            // todo
        }

        private void _Edit_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Delete_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Save_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_SelectType_Scalar_DataType_ComboBox.SelectedItem.ToString());
        }

    }
}
