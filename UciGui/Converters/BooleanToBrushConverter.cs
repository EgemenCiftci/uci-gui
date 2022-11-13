using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UciGui.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? b ? Brushes.Red : Brushes.Green : (object)Brushes.Green;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
