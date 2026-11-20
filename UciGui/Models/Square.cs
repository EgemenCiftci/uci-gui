using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UciGui.Models;

public class Square : INotifyPropertyChanged
{
    public Piece? Piece
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    public int Row { get; set; }

    public int Column { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
