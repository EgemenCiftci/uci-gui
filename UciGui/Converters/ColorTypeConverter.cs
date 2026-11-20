using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UciGui.Enums;

namespace UciGui.Converters;

public class ColorTypeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isInverse = System.Convert.ToBoolean(parameter);

        if (values != null &&
            values[0] is ColorTypes colorType &&
            values[1] is Brush lightBrush &&
            values[2] is Brush darkBrush)
        {
            switch (colorType)
            {
                case ColorTypes.Light:
                    return isInverse ? darkBrush : lightBrush;
                case ColorTypes.Dark:
                    return isInverse ? lightBrush : darkBrush;
            }
        }

        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
