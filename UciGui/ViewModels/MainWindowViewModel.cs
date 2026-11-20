using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UciGui.Models;
using UciGui.Services;

namespace UciGui.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public string? Fen
    {
        get;
        set => SetProperty(ref field, value);
    }
    public string? BestMove
    {
        get;
        set => SetProperty(ref field, value);
    }
    public string? Ponder
    {
        get;
        set => SetProperty(ref field, value);
    }
    public bool IsBusy
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Option> Options { get; } = [];

    public DelegateCommand GoCommand { get; }
    public DelegateCommand StopCommand { get; }
    public DelegateCommand<RoutedPropertyChangedEventArgs<double>> ValueChangedCommand { get; }
    public DelegateCommand<CheckBox> CheckBoxChangedCommand { get; }
    public DelegateCommand<RoutedEventArgs> TextChangedCommand { get; }
    public DelegateCommand<Option> ButtonChangedCommand { get; }
    public DelegateCommand<RoutedEventArgs> SelectionChangedCommand { get; }

    private readonly UciService _uciService;

    public MainWindowViewModel(UciService uciService)
    {
        _uciService = uciService;
        Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        GoCommand = new DelegateCommand(Go);
        StopCommand = new DelegateCommand(Stop);
        ValueChangedCommand = new DelegateCommand<RoutedPropertyChangedEventArgs<double>>(ValueChanged);
        CheckBoxChangedCommand = new DelegateCommand<CheckBox>(CheckBoxChanged);
        TextChangedCommand = new DelegateCommand<RoutedEventArgs>(TextChanged);
        ButtonChangedCommand = new DelegateCommand<Option>(ButtonChanged);
        SelectionChangedCommand = new DelegateCommand<RoutedEventArgs>(SelectionChanged);

        _uciService.Options?.ForEach(Options.Add);
    }

    private void ValueChanged(RoutedPropertyChangedEventArgs<double> e)
    {
        if (e.Source is FrameworkElement fe && fe.DataContext is Option option)
        {
            _uciService.SetOption(option, e.NewValue.ToString());
        }
    }

    private void CheckBoxChanged(CheckBox checkBox)
    {
        if (checkBox.DataContext is Option option)
        {
            _uciService.SetOption(option, checkBox.IsChecked.GetValueOrDefault().ToString());
        }
    }

    private void TextChanged(RoutedEventArgs e)
    {
        if (e.Source is TextBox tb && tb.DataContext is Option option)
        {
            _uciService.SetOption(option, tb.Text);
        }
    }

    private void ButtonChanged(Option option)
    {
        _uciService.SetOption(option, null);
    }

    private void SelectionChanged(RoutedEventArgs e)
    {
        if (e.Source is Selector s && s.DataContext is Option option)
        {
            _uciService.SetOption(option, s.SelectedItem.ToString());
        }
    }

    private void Go()
    {
        try
        {
            IsBusy = true;

            BestMove = null;
            Ponder = null;

            _uciService.NewGame();
            _uciService.SetPosition(Fen);
            _uciService.Go();
        }
        catch (Exception ex)
        {
            IsBusy = false;
            _ = MessageBox.Show(ex.Message);
        }
    }

    private void Stop()
    {
        try
        {
            IsBusy = false;

            _uciService.Stop();
            (string? bestMove, string? ponder) = _uciService.GetBestMoveAndPonder();

            BestMove = bestMove;
            Ponder = ponder;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            _ = MessageBox.Show(ex.Message);
        }
    }
}
