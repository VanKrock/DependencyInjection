namespace DependencyInjection
{
    public class MailService : IMailService
    {
        private readonly ILogService logService;

        public MailService(ILogService logService)
        {
            this.logService = logService;
        }

        public void Send(string message, string from, string to)
        {
            //TODO: реализовать отправку сообщений
            logService.Debug($@"Send email
                        From: {from}
                        To: {to}
                        {message}");
        }
    }
}
