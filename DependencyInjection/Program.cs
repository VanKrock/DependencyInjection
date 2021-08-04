using System;

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton(sp => new Options { EmailAddress = "no-replay@site" });
            services.AddTransient<ILogProvider, ConsoleLogProvider>();
            services.AddTransient<ConsoleLogProvider>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICustomFactory, CustomFactory>();

            var serviceProvider = services.BuildServiceProvider();
            var userService = serviceProvider.GetService<IUserService>();

            userService.Register("user@shmyandex", "1234");
        }

    }

    public interface ICustomFactory
    {
        ILogProvider Get(string value);
    }

    public class CustomFactory : ICustomFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CustomFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ILogProvider Get(string value) => value switch
        {
            "console" => serviceProvider.GetService<ConsoleLogProvider>(),
            _ => throw new NotSupportedException()
        };
    }
}
