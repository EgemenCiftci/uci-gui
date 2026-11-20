using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UciGui.Converters;

public class CellColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null &&
            values[0] is int columnIndex &&
            values[1] is int rowIndex)
        {
            bool result = System.Convert.ToBoolean(columnIndex % 2) == System.Convert.ToBoolean(rowIndex % 2);
            return new SolidColorBrush(result ? Color.FromRgb(240, 217, 181) : Color.FromRgb(118, 150, 86));
        }

        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
