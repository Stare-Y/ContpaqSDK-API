namespace Core.Domain.Interfaces.Services
{
    public interface ILogger
    {
        Task Log(string message);
    }
}
