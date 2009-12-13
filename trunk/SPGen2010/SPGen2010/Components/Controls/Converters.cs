using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace SPGen2010.Components.Controls.Converters
{
    public class BooleanToHiddenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rv = Visibility.Visible;
            try
            {
                var x = bool.Parse(value.ToString());
                if (x)
                {
                    rv = Visibility.Visible;
                }
                else
                {
                    rv = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    /// <summary>
    /// for sql_function type convert only
    /// todo: DPI calc or get datagrid's row template width & return
    /// </summary>
    public class FuncTypeToImageString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rv = "/SPGen2010;component/Images/sql_function_scale.png";
            try
            {
                if (value.GetType() == typeof(SPGen2010.Components.Modules.ObjectExplorer.UserDefinedFunction_Table))
                {
                    rv = "/SPGen2010;component/Images/sql_function_table.png";
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    /// <summary>
    /// for datagrid's row detail template width
    /// </summary>
    public class GetChildWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double rv = 200;
            try
            {
                double v = (double)value;
                rv = v - 110;
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
