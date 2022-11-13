using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UciGui.Converters;

public class BooleanToInverseVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? b ? Visibility.Collapsed : Visibility.Visible : (object)Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
