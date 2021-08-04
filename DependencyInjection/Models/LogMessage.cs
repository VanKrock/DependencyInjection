using System;

namespace DependencyInjection
{
    public class LogMessage
    {
        public LogMessageLevel Level { get; set; }

        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
