using System;
using System.Globalization;
using System.Windows.Data;

namespace UciGui.Converters;

public class BoardSizeToTextSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values != null && values[0] is double height && values[1] is double width ? Math.Min(height, width) / 32 : (object)0d;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
