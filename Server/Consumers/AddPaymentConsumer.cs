using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Server.Consumers
{
    public class AddPaymentConsumer : IConsumer<AddPaymentMessage>
    {
        readonly ILogger<AddPaymentConsumer> _logger;

        public AddPaymentConsumer(ILogger<AddPaymentConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AddPaymentMessage> context)
        {
            _logger.LogInformation(" => ID: {0}", context.Message.Id);

            var delay = Math.Min(int.Parse(context.Message.Id) * 200, 5000);
            await Task.Delay(delay);

            _logger.LogInformation(" <= ID: {0}", context.Message.Id);
        }
    }
}