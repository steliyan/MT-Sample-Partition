using System;

namespace Contracts
{
    public interface AddPaymentCompletedMessage
    {
        Guid CorrelationId { get; }
    }
}