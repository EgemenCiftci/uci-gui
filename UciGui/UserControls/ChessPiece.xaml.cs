using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UciGui.Enums;

namespace UciGui.UserControls;

public partial class ChessPiece : UserControl
{
    public PieceShapeTypes ShapeType
    {
        get => (PieceShapeTypes)GetValue(ShapeTypeProperty);
        set => SetValue(ShapeTypeProperty, value);
    }

    // Using a DependencyProperty as the backing store for ShapeType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShapeTypeProperty =
        DependencyProperty.Register("ShapeType", typeof(PieceShapeTypes), typeof(ChessPiece), new PropertyMetadata(PieceShapeTypes.Pawn));

    public PieceColorTypes ColorType
    {
        get => (PieceColorTypes)GetValue(ColorTypeProperty);
        set => SetValue(ColorTypeProperty, value);
    }

    // Using a DependencyProperty as the backing store for ColorType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColorTypeProperty =
        DependencyProperty.Register("ColorType", typeof(PieceColorTypes), typeof(ChessPiece), new PropertyMetadata(PieceColorTypes.Light));

    public Brush DarkBrush
    {
        get => (Brush)GetValue(DarkBrushProperty);
        set => SetValue(DarkBrushProperty, value);
    }

    // Using a DependencyProperty as the backing store for DarkBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DarkBrushProperty =
        DependencyProperty.Register("DarkBrush", typeof(Brush), typeof(ChessPiece), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    public Brush LightBrush
    {
        get => (Brush)GetValue(LightBrushProperty);
        set => SetValue(LightBrushProperty, value);
    }

    // Using a DependencyProperty as the backing store for LightBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LightBrushProperty =
        DependencyProperty.Register("LightBrush", typeof(Brush), typeof(ChessPiece), new PropertyMetadata(new SolidColorBrush(Colors.White)));

    public ChessPiece()
    {
        InitializeComponent();
    }
}
