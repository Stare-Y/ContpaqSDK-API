using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BasculaTerminalApi
{
    public class RandomNumberHub : Hub
    {
        private static Timer _timer = null!;
        private static IHubContext<RandomNumberHub> _context = null!;

        public RandomNumberHub(IHubContext<RandomNumberHub> context)
        {
            _context = context;
            if(_timer == null)
            {
                _timer = new Timer(3000);
                _timer.Elapsed += SendRandomNumber;
                _timer.Start();
            }
        }

        private void SendRandomNumber(object sender, ElapsedEventArgs e)
        {
            var randomNumber = new Random().Next(1, 100);
            _context.Clients.All.SendAsync("ReceiveNumber", randomNumber);
        }
    }
}
