using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Server.Consumers
{
    public class ProcessPaymentConsumer : IConsumer<ProcessPaymentMessage>
    {
        readonly ILogger<ProcessPaymentConsumer> _logger;

        public ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessPaymentMessage> context)
        {
            _logger.LogInformation($" => ID: {context.Message.Id} (Amount: {context.Message.Amount})");

            var delay = Math.Min(int.Parse(context.Message.Id) * 200, 5000);
            await Task.Delay(delay);

            _logger.LogInformation($" <= ID: {context.Message.Id} (Amount: {context.Message.Amount})");

            await context.Publish<AddPaymentCompletedMessage>(context.Message);
        }
    }
}