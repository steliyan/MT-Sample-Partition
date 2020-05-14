using System;

namespace Contracts
{
    public interface ProcessPaymentMessage
    {
        Guid CorrelationId { get; }

        string Id { get; }

        decimal Amount { get; }
    }
}