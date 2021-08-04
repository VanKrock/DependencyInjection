namespace DependencyInjection
{
    public class UserService : IUserService
    {
        private readonly IMailService mailService;
        private readonly ILogService logService;
        private readonly Options options;

        public UserService(IMailService mailService, Options options)
        {
            this.mailService = mailService;
            //this.logService = logService;
            this.options = options;
        }

        public void Register(string email, string password)
        {
            //logService.Info($"Register user {email}");
            //...
            mailService.Send("Welcome to site", options.EmailAddress, email);
            //logService.Info($"User {email} successfully registered");
        }
    }
}
