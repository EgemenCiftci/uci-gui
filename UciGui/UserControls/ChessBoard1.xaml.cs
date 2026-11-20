using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UciGui.Enums;
using UciGui.Models;

namespace UciGui.UserControls;

public partial class ChessBoard1 : UserControl
{
    public string Fen
    {
        get => (string)GetValue(FenProperty);
        set => SetValue(FenProperty, value);
    }

    public static readonly DependencyProperty FenProperty =
        DependencyProperty.Register(
            nameof(Fen),
            typeof(string),
            typeof(ChessBoard1),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFenChanged));

    private Point _dragStartPoint;
    private Square? _draggedSquare;
    private Piece? _draggedPiece;
    private bool _draggedPieceIsCaptured;
    private bool _updatingFenFromBoard;

    private static void OnFenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not ChessBoard1 board)
        {
            return;
        }

        if (!board._updatingFenFromBoard)
        {
            board.CapturedPieces.Clear();
        }

        foreach (Square square in board.Squares)
        {
            square.Piece = null;
        }

        if (e.NewValue is string fen)
        {
            ParseFen(board.Squares, fen);
        }
    }


    public ObservableCollection<Square> Squares { get; } = [with(
        Enumerable.Range(0, 64).Select(index => new Square
        {
            Row = index / 8,
            Column = index % 8
        }))];

    public ObservableCollection<Piece> AvailablePieces { get; } = [with(
        Enum.GetValues<PieceShapeTypes>()
            .SelectMany(shape => new[]
            {
                new Piece { ShapeType = shape, ColorType = ColorTypes.Light },
                new Piece { ShapeType = shape, ColorType = ColorTypes.Dark }
            }))];

    public ObservableCollection<Piece> CapturedPieces { get; } = [];

    public ChessBoard1()
    {
        InitializeComponent();
    }

    private void Square_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(this);
        _draggedSquare = (sender as FrameworkElement)?.DataContext as Square;
    }

    private void Square_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _draggedSquare?.Piece is null)
        {
            return;
        }

        Point position = e.GetPosition(this);
        if (Math.Abs(position.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(position.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        Piece piece = _draggedSquare.Piece;
        _ = DragDrop.DoDragDrop((DependencyObject)sender, new PieceDragData(piece, _draggedSquare, false), DragDropEffects.Move);
        _draggedSquare = null;
    }

    private void PalettePiece_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        StartPieceDrag(sender, e, isCaptured: false);
    }

    private void CapturedPiece_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        StartPieceDrag(sender, e, isCaptured: true);
    }

    private void StartPieceDrag(object sender, MouseButtonEventArgs e, bool isCaptured)
    {
        _dragStartPoint = e.GetPosition(this);
        _draggedPiece = (sender as FrameworkElement)?.DataContext as Piece;
        _draggedPieceIsCaptured = isCaptured;
    }

    private void PieceSource_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _draggedPiece is null)
        {
            return;
        }

        Point position = e.GetPosition(this);
        if (Math.Abs(position.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(position.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        _ = DragDrop.DoDragDrop((DependencyObject)sender,
            new PieceDragData(_draggedPiece, null, _draggedPieceIsCaptured),
            _draggedPieceIsCaptured ? DragDropEffects.Move : DragDropEffects.Copy);
        _draggedPiece = null;
    }

    private void Square_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetData(typeof(PieceDragData)) is PieceDragData dragData
            ? dragData.SourceSquare is null && !dragData.IsCaptured
                ? DragDropEffects.Copy
                : DragDropEffects.Move
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void Square_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(typeof(PieceDragData)) is not PieceDragData dragData ||
            (sender as FrameworkElement)?.DataContext is not Square targetSquare ||
            (dragData.SourceSquare is not null && ReferenceEquals(dragData.SourceSquare, targetSquare)))
        {
            return;
        }

        if (targetSquare.Piece is not null)
        {
            CapturedPieces.Add(targetSquare.Piece);
        }

        targetSquare.Piece = dragData.Piece;
        if (dragData.SourceSquare is not null)
        {
            dragData.SourceSquare.Piece = null;
        }
        else if (dragData.IsCaptured)
        {
            _ = CapturedPieces.Remove(dragData.Piece);
        }

        UpdateFenFromBoard();
        e.Handled = true;
    }

    private void CapturedTray_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetData(typeof(PieceDragData)) is PieceDragData dragData &&
                    dragData.SourceSquare is not null
            ? DragDropEffects.Move
            : DragDropEffects.None;
        e.Handled = true;
    }

    private void CapturedTray_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(typeof(PieceDragData)) is not PieceDragData dragData ||
            dragData.SourceSquare is null)
        {
            return;
        }

        dragData.SourceSquare.Piece = null;
        CapturedPieces.Add(dragData.Piece);
        UpdateFenFromBoard();
        e.Handled = true;
    }

    private void UpdateFenFromBoard()
    {
        _updatingFenFromBoard = true;
        try
        {
            SetCurrentValue(FenProperty, CreateFen());
        }
        finally
        {
            _updatingFenFromBoard = false;
        }
    }

    private sealed record PieceDragData(Piece Piece, Square? SourceSquare, bool IsCaptured);

    private string CreateFen()
    {
        string[] fields = string.IsNullOrWhiteSpace(Fen)
            ? Array.Empty<string>()
            : Fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string placement = string.Join('/', Enumerable.Range(0, 8).Select(row =>
        {
            int emptySquares = 0;
            string rank = string.Empty;

            foreach (Square square in Squares.Where(square => square.Row == row).OrderBy(square => square.Column))
            {
                if (square.Piece is null)
                {
                    emptySquares++;
                    continue;
                }

                if (emptySquares > 0)
                {
                    rank += emptySquares;
                    emptySquares = 0;
                }

                char piece = square.Piece.ShapeType switch
                {
                    PieceShapeTypes.Rook => 'r',
                    PieceShapeTypes.Knight => 'n',
                    PieceShapeTypes.Bishop => 'b',
                    PieceShapeTypes.Queen => 'q',
                    PieceShapeTypes.King => 'k',
                    PieceShapeTypes.Pawn => 'p',
                    _ => throw new ArgumentOutOfRangeException(nameof(square.Piece.ShapeType))
                };
                rank += square.Piece.ColorType == ColorTypes.Light ? char.ToUpperInvariant(piece) : piece;
            }

            if (emptySquares > 0)
            {
                rank += emptySquares;
            }

            return rank;
        }));

        if (fields.Length == 0)
        {
            return $"{placement} w - - 0 1";
        }

        fields[0] = placement;
        return string.Join(' ', fields);
    }

    public static void ParseFen(ObservableCollection<Square> squares, string fen)
    {
        string? board = fen.Split(' ').FirstOrDefault();
        if (board is null)
        {
            return;
        }

        string[] rows = board.Split('/');
        if (rows.Length != 8 || squares.Count != 64)
        {
            return;
        }

        Piece?[] pieces = new Piece?[64];
        for (int row = 0; row < rows.Length; row++)
        {
            int column = 0;
            foreach (char letter in rows[row])
            {
                if (letter is >= '1' and <= '8')
                {
                    column += letter - '0';
                }
                else
                {
                    PieceShapeTypes shape = letter switch
                    {
                        'R' or 'r' => PieceShapeTypes.Rook,
                        'N' or 'n' => PieceShapeTypes.Knight,
                        'B' or 'b' => PieceShapeTypes.Bishop,
                        'Q' or 'q' => PieceShapeTypes.Queen,
                        'K' or 'k' => PieceShapeTypes.King,
                        'P' or 'p' => PieceShapeTypes.Pawn,
                        _ => (PieceShapeTypes)(-1)
                    };

                    if (shape == (PieceShapeTypes)(-1) || column >= 8)
                    {
                        return;
                    }

                    pieces[(row * 8) + column] = new Piece
                    {
                        ColorType = char.IsUpper(letter) ? ColorTypes.Light : ColorTypes.Dark,
                        ShapeType = shape
                    };
                    column++;
                }

                if (column > 8)
                {
                    return;
                }
            }

            if (column != 8)
            {
                return;
            }
        }

        for (int index = 0; index < pieces.Length; index++)
        {
            squares[index].Piece = pieces[index];
        }
    }
}
