using System.Windows;
using System.Windows.Controls;
using UciGui.UserControls;

namespace UciGui.TemplateSelectors;

public class ChessPieceDataTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (container is ContentPresenter cp &&
            cp.TemplatedParent is ContentControl cc &&
            cc.Parent is Viewbox vb &&
            vb.Parent is ChessPiece piece)
        {
            switch (piece.ShapeType)
            {
                case Enums.PieceShapeTypes.Pawn:
                    return (DataTemplate)piece.FindResource("pawn");
                case Enums.PieceShapeTypes.Knight:
                    return (DataTemplate)piece.FindResource("knight");
                case Enums.PieceShapeTypes.Bishop:
                    return (DataTemplate)piece.FindResource("bishop");
                case Enums.PieceShapeTypes.Rook:
                    return (DataTemplate)piece.FindResource("rook");
                case Enums.PieceShapeTypes.Queen:
                    return (DataTemplate)piece.FindResource("queen");
                case Enums.PieceShapeTypes.King:
                    return (DataTemplate)piece.FindResource("king");
            }
        }

        return base.SelectTemplate(item, container);
    }
}
