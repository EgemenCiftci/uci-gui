using System;
using System.Globalization;
using System.Windows.Data;
using UciGui.Enums;

namespace UciGui.Converters;

public class ShapeTypeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values != null &&
            values[0] is PieceShapeTypes shapeType &&
            values[1] is ColorTypes colorType
            ? shapeType switch
            {
                PieceShapeTypes.Pawn => colorType == ColorTypes.Light ? "♙" : "♟︎",
                PieceShapeTypes.Knight => colorType == ColorTypes.Light ? "♘" : "♞",
                PieceShapeTypes.Bishop => colorType == ColorTypes.Light ? "♗" : "♝",
                PieceShapeTypes.Rook => colorType == ColorTypes.Light ? "♖" : "♜",
                PieceShapeTypes.Queen => colorType == ColorTypes.Light ? "♕" : "♛",
                PieceShapeTypes.King => colorType == ColorTypes.Light ? "♔" : "♚",
                _ => string.Empty,
            }
            : string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
