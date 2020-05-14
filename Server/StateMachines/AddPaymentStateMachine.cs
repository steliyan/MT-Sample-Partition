using System.Collections.Generic;
using Automatonymous;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Server.StateMachines
{
    public class AddPaymentStateMachine : MassTransitStateMachine<AddPaymentState>
    {
        private readonly ILogger<AddPaymentStateMachine> _logger;

        public AddPaymentStateMachine(ILogger<AddPaymentStateMachine> logger)
        {
            _logger = logger;

            InstanceState(instance => instance.CurrentState);

            Event(() => PaymentRequested, x =>
            {
                x.CorrelateById(m => m.Message.CorrelationId);

                x.SetSagaFactory(ctx => new AddPaymentState
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    PaymentId = ctx.Message.Id,
                    PaymentQueue = new Queue<PaymentMessage>(new[] {ctx.Message}),
                });
            });

            Event(() => AddPaymentCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => ProcessPaymentRequested, x => x.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(PaymentRequested)
                    .TransitionTo(Requested)
                    .PublishAsync(x => x.Init<ProcessPaymentRequestMessage>(new {x.Data.CorrelationId}))
            );

            During(Requested,
                When(ProcessPaymentRequested)
                    .PublishAsync(context => context.Init<ProcessPaymentMessage>(context.Instance.PaymentQueue.Peek())),
                When(PaymentRequested)
                    .Then(ctx => ctx.Instance.PaymentQueue.Enqueue(ctx.Data)),
                When(AddPaymentCompleted)
                    .IfElse(
                        ctx => ctx.Instance.PaymentQueue.Count == 1,
                        x => x
                            .Then(ctx => _logger.LogInformation($"COMPLETING SAGA: {ctx.Instance.PaymentId}"))
                            .Finalize(),
                        x => x
                            .Then(ctx => ctx.Instance.PaymentQueue.Dequeue())
                            .PublishAsync(ctx =>
                            {
                                var payment = ctx.Instance.PaymentQueue.Peek();
                                return ctx.Init<ProcessPaymentRequestMessage>(new {payment.CorrelationId});
                            }))
            );

            SetCompletedWhenFinalized();
        }

        public State Requested { get; private set; }

        public Event<PaymentMessage> PaymentRequested { get; private set; }

        public Event<AddPaymentCompletedMessage> AddPaymentCompleted { get; private set; }

        public Event<ProcessPaymentRequestMessage> ProcessPaymentRequested { get; private set; }
    }
}