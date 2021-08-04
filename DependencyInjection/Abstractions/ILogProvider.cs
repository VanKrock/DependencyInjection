namespace DependencyInjection
{
    public interface ILogProvider
    {
        void Send(LogMessage message);
    }
}
