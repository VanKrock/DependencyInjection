namespace DependencyInjection
{
    public interface IMailService
    {
        void Send(string message, string from, string to);
    }
}
