namespace DependencyInjection
{
    public interface ILogService
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
    }
}
