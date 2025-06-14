using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public abstract record BaseEvent
(
    Guid Id,
    DateTime Timestamp,
    string Account,
    Decimal Amount
)
{
    public virtual string EventName => GetType().Name;
    public string? ReversedEvent { get; set; }
}
