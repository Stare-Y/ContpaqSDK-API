using Core.Domain.Interfaces.Services;

namespace Infrastructure.Services
{
    public class Logger : ILogger
    {
        private readonly string _path;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public Logger(string path)
        {
            _path = path;
        }

        public async Task Log(string message)
        {
            await _semaphore.WaitAsync();

            try
            {
                File.AppendAllText(_path, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing log: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
