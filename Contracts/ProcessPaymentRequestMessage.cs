using System;

namespace Contracts
{
    public interface ProcessPaymentRequestMessage
    {
        Guid CorrelationId { get; }
    }
}
