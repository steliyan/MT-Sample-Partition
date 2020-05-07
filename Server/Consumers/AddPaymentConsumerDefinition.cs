using Contracts;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace Server.Consumers
{
    public class AddPaymentConsumerDefinition : ConsumerDefinition<AddPaymentConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddPaymentConsumer> consumerConfigurator)
        {
            consumerConfigurator.Message<AddPaymentMessage>(x => x.UsePartitioner(8, m => m.Message.Id));
        }
    }
}