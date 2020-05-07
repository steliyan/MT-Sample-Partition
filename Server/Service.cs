using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public class Service : IHostedService
    {
        private readonly IBusControl _bus;

        public Service(IBusControl bus)
        {
            _bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _bus.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }
    }
}