using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public record ReversalEvent(
    Guid Id,
    DateTime Timestamp,
    string Account,
    decimal Amount,
    Guid OriginalEventId,
    string OriginalEventName
) : BaseEvent(Id, Timestamp, Account, Amount)
{
    public override string EventName => $"{OriginalEventName}Reversed";
}
