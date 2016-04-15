﻿using chess.delikirpi.com.Models;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace chess.delikirpi.com.Controllers
{
    public class ChessController : ApiController
    {
        Process process;

        // GET api/values/5
        public Response Get(string fen, bool isChess960 = false)
        {
            try
            {
                if (!IsFenValid(fen))
                    return new Response { Code = -1, Description = "FEN is not valid.", Fen = fen, IsChess960 = isChess960 };

                fen = fen.Trim();

                // Create process start information
                var processStartInfo = new ProcessStartInfo(HttpRuntime.AppDomainAppPath + "stockfish 7 x64 bmi2.exe");
                processStartInfo.UseShellExecute = false;
                processStartInfo.ErrorDialog = false;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                // Start process
                process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();

                process.StandardInput.WriteLine("setoption name UCI_Chess960 value " + isChess960);
                process.StandardInput.WriteLine("setoption name Threads value 4");
                process.StandardInput.WriteLine("ucinewgame");
                process.StandardInput.WriteLine("position fen {0}", fen);
                process.StandardInput.WriteLine("go infinite");

                System.Threading.Thread.Sleep(10000);

                process.StandardInput.WriteLine("stop");

                while (true)
                {
                    var line = process.StandardOutput.ReadLine();

                    if (line != null && line.StartsWith("bestmove"))
                    {
                        var bestMove = GetWord(line, "bestmove", "ponder");
                        var ponder = GetWord(line, "ponder", null);

                        process.StandardInput.WriteLine("quit");

                        return new Response { Fen = fen, IsChess960 = isChess960, BestMove = bestMove, Ponder = ponder };
                    }
                }
            }
            catch (Exception ex)
            {
                if (process != null && process.StandardInput != null)
                    process.StandardInput.WriteLine("quit");

                return new Response { Code = -1, Description = ex.Message, Fen = fen, IsChess960 = isChess960 };
            }
        }

        private bool IsFenValid(string fen)
        {
            if (string.IsNullOrWhiteSpace(fen))
                return false;

            return Regex.IsMatch(fen, @"([rnbqkpRNBQKP1-8]+\/){7}([rnbqkpRNBQKP1-8]+)\s[bw-]\s(([a-hkqA-HKQ]{1,4})|(-))\s(([a-h][36])|(-))\s(0|[1-9][0-9]*)\s([1-9][0-9]*)");
        }

        private string GetWord(string optionLine, string previousWord, string nextWord)
        {
            var previousWordIndex = optionLine.IndexOf(previousWord);

            if (previousWordIndex < 0)
            {
                return null;
            }
            else
            {
                var startIndex = previousWordIndex + previousWord.Length + 1;

                if (nextWord == null)
                {
                    return optionLine.Substring(startIndex);
                }
                else
                {
                    var nextWordIndex = optionLine.IndexOf(nextWord);

                    if (nextWordIndex < 0)
                    {
                        return optionLine.Substring(startIndex);
                    }
                    else
                    {
                        var length = nextWordIndex - 1 - startIndex;

                        return optionLine.Substring(startIndex, length);
                    }
                }
            }
        }
    }
}
