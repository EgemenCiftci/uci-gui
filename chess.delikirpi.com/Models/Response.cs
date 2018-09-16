using System;

namespace chess.delikirpi.com.Models
{
    public class Response
    {
        public int Code { get; set; }

        public string Description { get; set; }

        public string Fen { get; set; }

        public bool IsChess960 { get; set; }

        public string BestMove { get; set; }

        public string Ponder { get; set; }

        public int ThinkDurationInSeconds { get; set; }

        public DateTime DateTime { get; set; }
    }
}