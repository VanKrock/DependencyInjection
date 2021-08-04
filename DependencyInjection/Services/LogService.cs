using System;
using System.Collections.Generic;

namespace DependencyInjection
{
    public class LogService : ILogService
    {
        private readonly IEnumerable<ILogProvider> logProviders;

        public LogService(IEnumerable<ILogProvider> logProviders)
        {
            this.logProviders = logProviders;
        }

        public void Debug(string message) => Send(message, LogMessageLevel.Debug);

        public void Info(string message) => Send(message, LogMessageLevel.Info);

        public void Error(string message) => Send(message, LogMessageLevel.Error);

        private void Send(string message, LogMessageLevel level)
        {
            var logMessage = new LogMessage
            {
                Level = level,
                Date = DateTime.Now,
                Text = message
            };
            foreach (var provider in logProviders)
            {
                provider.Send(logMessage);
            }
        }
    }
}
