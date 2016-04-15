namespace UciGui
{
    public class Option
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Default { get; set; }

        public int Minimum { get; set; }

        public int Maximum { get; set; }

        public string[] Items { get; set; }
    }
}
