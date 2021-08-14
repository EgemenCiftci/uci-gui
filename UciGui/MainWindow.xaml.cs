using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UciGui.Enums;
using UciGui.Properties;

namespace UciGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string[] keywords = new[] { "name", "type", "default", "min", "max", "var", "bestmove", "ponder" };

        public string Fen
        {
            get => (string)GetValue(FenProperty);
            set => SetValue(FenProperty, value);
        }

        // Using a DependencyProperty as the backing store for Fen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FenProperty =
            DependencyProperty.Register("Fen", typeof(string), typeof(MainWindow), new PropertyMetadata(null));


        public UIElement[] Options
        {
            get => (UIElement[])GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Options.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(UIElement[]), typeof(MainWindow), new PropertyMetadata(null));


        public string BestMove
        {
            get => (string)GetValue(BestMoveProperty);
            set => SetValue(BestMoveProperty, value);
        }

        // Using a DependencyProperty as the backing store for BestMove.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BestMoveProperty =
            DependencyProperty.Register("BestMove", typeof(string), typeof(MainWindow), new PropertyMetadata(null));


        public string Ponder
        {
            get => (string)GetValue(PonderProperty);
            set => SetValue(PonderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Ponder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PonderProperty =
            DependencyProperty.Register("Ponder", typeof(string), typeof(MainWindow), new PropertyMetadata(null));


        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        private Process process;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create process start information
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Settings.Default.UciEngineExe)
                {
                    UseShellExecute = false,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                // Start process
                process = new Process
                {
                    StartInfo = processStartInfo
                };
                bool processStarted = process.Start();

                if (processStarted)
                {
                    List<string> optionLines = GetOptionLines(process);

                    UpdateGuiOptions(optionLines);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        private void UpdateGuiOptions(List<string> optionLines)
        {
            List<Option> options = optionLines.Select(f => GetDict("option", f)).Select(f => new Option
            {
                Name = f.ContainsKey("name") ? f["name"] : null,
                Type = f.ContainsKey("type") ? f["type"] : OptionTypes.None,
                Default = f.ContainsKey("default") ? f["default"] : null,
                Minimum = f.ContainsKey("min") ? f["min"] : 0,
                Maximum = f.ContainsKey("max") ? f["max"] : 0,
                Items = f.ContainsKey("var") ? f["var"].ToArray() : null,
            }).ToList();

            List<UIElement> o = new List<UIElement>();

            foreach (Option option in options)
            {
                switch (option.Type)
                {
                    case OptionTypes.Spin:
                        DockPanel dp = new DockPanel { Margin = new Thickness(2) };
                        _ = dp.Children.Add(new TextBlock { Text = option.Name + ": ", VerticalAlignment = VerticalAlignment.Center });
                        Slider sld = new Slider { Value = Convert.ToInt32(option.Default), Minimum = option.Minimum, Maximum = option.Maximum, SmallChange = 1, IsSnapToTickEnabled = true, AutoToolTipPlacement = AutoToolTipPlacement.TopLeft };
                        sld.ValueChanged += (s, e) =>
                        {
                            SetOption(option, sld.Value.ToString());
                        };
                        DockPanel.SetDock(sld, Dock.Right);
                        _ = dp.Children.Add(sld);
                        o.Add(dp);
                        break;
                    case OptionTypes.Check:
                        CheckBox cb = new CheckBox { Content = option.Name, IsChecked = Convert.ToBoolean(option.Default), Margin = new Thickness(2) };
                        cb.Checked += (s, e) =>
                        {
                            SetOption(option, "true");
                        };
                        cb.Unchecked += (s, e) =>
                        {
                            SetOption(option, "false");
                        };
                        o.Add(cb);
                        break;
                    case OptionTypes.String:
                        DockPanel dp1 = new DockPanel { Margin = new Thickness(2) };
                        _ = dp1.Children.Add(new TextBlock { Text = option.Name + ": ", VerticalAlignment = VerticalAlignment.Center });
                        TextBox wtb = new TextBox
                        {
                            Text = option.Default,
                            Margin = new Thickness(2)
                        };
                        wtb.TextChanged += (s, e) =>
                        {
                            SetOption(option, wtb.Text);
                        };
                        DockPanel.SetDock(wtb, Dock.Right);
                        _ = dp1.Children.Add(wtb);
                        o.Add(dp1);
                        break;
                    case OptionTypes.Button:
                        Button btn = new Button { Content = option.Name, Margin = new Thickness(2) };
                        btn.Click += (s, e) =>
                        {
                            SetOption(option, null);
                        };
                        o.Add(btn);
                        break;
                    case OptionTypes.Combo:
                        DockPanel dp0 = new DockPanel { Margin = new Thickness(2) };
                        _ = dp0.Children.Add(new TextBlock { Text = option.Name + ": ", VerticalAlignment = VerticalAlignment.Center });
                        ComboBox cbx = new ComboBox { ItemsSource = option.Items, SelectedItem = option.Default, Margin = new Thickness(2) };
                        cbx.SelectionChanged += (s, e) =>
                        {
                            SetOption(option, (string)cbx.SelectedItem);
                        };
                        DockPanel.SetDock(cbx, Dock.Right);
                        _ = dp0.Children.Add(cbx);
                        o.Add(dp0);
                        break;
                    case OptionTypes.None:
                    default:
                        break;
                }
            }

            Options = o.ToArray();
        }

        private List<string> GetOptionLines(Process process)
        {
            process.StandardInput.WriteLine("uci");

            List<string> options = new List<string>();

            while (true)
            {
                string line = process.StandardOutput.ReadLine();

                if (line != null)
                {
                    if (line.StartsWith("option"))
                    {
                        options.Add(line);
                    }
                    else if (line.StartsWith("uciok"))
                    {
                        return options;
                    }
                }
            }
        }

        private void SetOption(Option option, string newValue)
        {
            if (newValue == null)
            {
                process.StandardInput.WriteLine("setoption name {0}", option.Name);
            }
            else
            {
                process.StandardInput.WriteLine("setoption name {0} value {1}", option.Name, newValue);
            }
        }

        private Dictionary<string, dynamic> GetDict(string type, string line)
        {
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

            if (type != null)
            {
                line = line.Substring(line.IndexOf(type) + type.Length + 1);
            }

            string[] words = line.Split(' ').Select(f => f.Trim()).ToArray();

            string key = null;
            List<string> values = new List<string>();
            for (int i = 0; i < words.Length; i++)
            {
                if (keywords.Any(f => f == words[i]))
                {
                    if (!string.IsNullOrWhiteSpace(key) && values.Count > 0)
                    {
                        AddToDict(dict, key, values);
                    }

                    key = words[i];
                    values.Clear();
                }
                else
                {
                    values.Add(words[i]);
                }
            }

            if (!string.IsNullOrWhiteSpace(key) && values.Count > 0)
            {
                AddToDict(dict, key, values);
            }

            return dict;
        }

        private static void AddToDict(Dictionary<string, dynamic> dict, string key, List<string> values)
        {
            string value = string.Join(" ", values);

            if (key == "min" || key == "max")
            {
                dict.Add(key, Convert.ToInt32(value));
            }
            else if (key == "type")
            {
                dict.Add(key, Enum.Parse(typeof(OptionTypes), char.ToUpper(value[0]) + value.Substring(1)));
            }
            else if (key == "var")
            {
                if (dict.ContainsKey("var"))
                {
                    dict["var"].Add(value);
                }
                else
                {
                    dict.Add(key, new List<string>(new string[] { value }));
                }
            }
            else
            {
                dict.Add(key, value);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ToggleButton)sender).Content = "STOP";
                IsBusy = true;

                BestMove = null;
                Ponder = null;

                process.StandardInput.WriteLine("ucinewgame");

                Fen = Fen?.Trim();

                if (string.IsNullOrWhiteSpace(Fen))
                {
                    process.StandardInput.WriteLine("position startpos");
                }
                else
                {
                    process.StandardInput.WriteLine("position fen {0}", Fen);
                }

                process.StandardInput.WriteLine("go infinite");
            }
            catch (Exception ex)
            {
                IsBusy = false;
                _ = MessageBox.Show(ex.Message);
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ToggleButton)sender).Content = "GO";
                IsBusy = false;

                process.StandardInput.WriteLine("stop");

                while (true)
                {
                    string line = process.StandardOutput.ReadLine();

                    if (line != null && line.StartsWith("bestmove"))
                    {
                        Dictionary<string, dynamic> dict = GetDict(null, line);

                        BestMove = dict.ContainsKey("bestmove") ? dict["bestmove"] : null;
                        Ponder = dict.ContainsKey("ponder") ? dict["ponder"] : null;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                MessageBox.Show(ex.Message);
            }
        }

        private void ParseFen(string fen)
        {
            if (!IsFenValid(fen))
            {
                return;
            }
        }

        private bool IsFenValid(string fen)
        {
            return !string.IsNullOrWhiteSpace(fen)
                && Regex.IsMatch(fen, @"([rnbqkpRNBQKP1-8]+\/){7}([rnbqkpRNBQKP1-8]+)\s[bw-]\s(([a-hkqA-HKQ]{1,4})|(-))\s(([a-h][36])|(-))\s(0|[1-9][0-9]*)\s([1-9][0-9]*)");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(IsFenValid(Fen));
        }
    }
}
