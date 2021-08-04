using System;

namespace DependencyInjection
{
    public class ConsoleLogProvider : ILogProvider
    {
        public void Send(LogMessage message)
        {
            Console.WriteLine($"[{message.Level}] {message.Date:HH:mm:ss} {message.Text}");
        }
    }
}
