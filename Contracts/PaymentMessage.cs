using System;

namespace Contracts
{
    public interface PaymentMessage
    {
        Guid CorrelationId { get; }

        string Id { get; }

        decimal Amount { get; }
    }
}