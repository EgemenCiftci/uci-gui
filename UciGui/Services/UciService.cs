using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UciGui.Enums;
using UciGui.Models;
using UciGui.Properties;

namespace UciGui.Services
{
    public class UciService
    {
        private readonly string[] keywords = new[] { "name", "type", "default", "min", "max", "var", "bestmove", "ponder" };
        private readonly Process _process = new() { StartInfo = GetProcessStartInfo() };
        public List<Option>? Options;
        public bool IsProcessStarted;

        public UciService()
        {
            IsProcessStarted = _process.Start();

            if (IsProcessStarted)
            {
                Options = GetOptions();
            }
        }

        private static ProcessStartInfo GetProcessStartInfo()
        {
            return new(Settings.Default.UciEngineExe)
            {
                UseShellExecute = false,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
        }

        private List<string> GetOptionLines()
        {
            List<string> options = new();

            _process.StandardInput.WriteLine("uci");

            while (true)
            {
                string? line = _process.StandardOutput.ReadLine();

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

        private List<Option> GetOptions()
        {
            return GetOptionLines().Select(f => GetDict("option", f)).Select(f => new Option
            {
#pragma warning disable IDE0008
                Name = f.TryGetValue("name", out var v0) ? v0 : null,
#pragma warning disable IDE0008
                Type = f.TryGetValue("type", out var v1) ? v1 : OptionTypes.None,
#pragma warning disable IDE0008
                Default = f.TryGetValue("default", out var v2) ? v2 : null,
#pragma warning disable IDE0008
                Minimum = f.TryGetValue("min", out var v3) ? v3 : 0,
#pragma warning disable IDE0008
                Maximum = f.TryGetValue("max", out var v4) ? v4 : 0,
#pragma warning disable IDE0008
                Items = f.TryGetValue("var", out var v5) ? v5.ToArray() : null,
            }).ToList();
        }

        public void SetOption(Option option, string? newValue)
        {
            _process.StandardInput.WriteLine("setoption name {0}{1}", option.Name, newValue == null ? null : $" value {newValue}");
        }

        private Dictionary<string, dynamic> GetDict(string? type, string line)
        {
            Dictionary<string, dynamic> dict = new();

            if (type != null)
            {
                line = line[(line.IndexOf(type) + type.Length + 1)..];
            }

            string[] words = line.Split(' ').Select(f => f.Trim()).ToArray();

            string? key = null;
            List<string> values = new();
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

            if (key is "min" or "max")
            {
                dict.Add(key, Convert.ToInt32(value));
            }
            else if (key == "type")
            {
                dict.Add(key, Enum.Parse(typeof(OptionTypes), char.ToUpper(value[0]) + value[1..]));
            }
            else if (key == "var")
            {
#pragma warning disable IDE0008
                if (dict.TryGetValue("var", out var v0))
                {
                    v0.Add(value);
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

        public void NewGame()
        {
            _process.StandardInput.WriteLine("ucinewgame");
        }

        public void SetPosition(string? fen)
        {
            _process.StandardInput.WriteLine("position {0}", string.IsNullOrWhiteSpace(fen) ? "startpos" : $"fen {fen.Trim()}");
        }

        public void Go()
        {
            _process.StandardInput.WriteLine("go infinite");
        }

        public void Stop()
        {
            _process.StandardInput.WriteLine("stop");
        }

        public (string? bestMove, string? ponder) GetBestMoveAndPonder()
        {
            while (true)
            {
                string? line = _process.StandardOutput.ReadLine();

                if (line != null && line.StartsWith("bestmove"))
                {
                    Dictionary<string, dynamic> dict = GetDict(null, line);
#pragma warning disable IDE0008
                    string? bestMove = dict.TryGetValue("bestmove", out var v0) ? v0 : null;
#pragma warning disable IDE0008
                    string? ponder = dict.TryGetValue("ponder", out var v1) ? v1 : null;
                    return (bestMove, ponder);
                }
            }
        }
    }
}
