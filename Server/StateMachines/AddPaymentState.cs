using System;
using System.Collections.Generic;
using Automatonymous;
using Contracts;
using MassTransit.RedisIntegration;

namespace Server.StateMachines
{
    public class AddPaymentState : SagaStateMachineInstance, IVersionedSaga
    {
        public Guid CorrelationId { get; set; }

        public string PaymentId { get; set; }

        public Queue<PaymentMessage> PaymentQueue { get; set; }

        public string CurrentState { get; set; }

        public int Version { get; set; }
    }
}
