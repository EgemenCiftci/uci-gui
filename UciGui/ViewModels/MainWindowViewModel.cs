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
    private string? _fen;
    public string? Fen
    {
        get => _fen;
        set => SetProperty(ref _fen, value);
    }

    private string? _bestMove;
    public string? BestMove
    {
        get => _bestMove;
        set => SetProperty(ref _bestMove, value);
    }

    private string? _ponder;
    public string? Ponder
    {
        get => _ponder;
        set => SetProperty(ref _ponder, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<Option> Options { get; } = new ObservableCollection<Option>();

    public DelegateCommand<bool?> GoStopCommand { get; }
    public DelegateCommand<RoutedPropertyChangedEventArgs<double>> ValueChangedCommand { get; }
    public DelegateCommand<CheckBox> CheckBoxChangedCommand { get; }
    public DelegateCommand<RoutedEventArgs> TextChangedCommand { get; }
    public DelegateCommand<Option> ButtonChangedCommand { get; }
    public DelegateCommand<RoutedEventArgs> SelectionChangedCommand { get; }

    private readonly UciService _uciService;

    public MainWindowViewModel(UciService uciService)
    {
        _uciService = uciService;

        GoStopCommand = new DelegateCommand<bool?>(GoStop);
        ValueChangedCommand = new DelegateCommand<RoutedPropertyChangedEventArgs<double>>(ValueChanged);
        CheckBoxChangedCommand = new DelegateCommand<CheckBox>(CheckBoxChanged);
        TextChangedCommand = new DelegateCommand<RoutedEventArgs>(TextChanged);
        ButtonChangedCommand = new DelegateCommand<Option>(ButtonChanged);
        SelectionChangedCommand = new DelegateCommand<RoutedEventArgs>(SelectionChanged);

        _uciService.Options?.ForEach(Options.Add);
    }


    private void GoStop(bool? isChecked)
    {
        if (isChecked == null)
        {
            return;
        }

        if (isChecked.Value)
        {
            Go();
        }
        else
        {
            Stop();
        }
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
            (string bestMove, string ponder) = _uciService.GetBestMoveAndPonder();

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
