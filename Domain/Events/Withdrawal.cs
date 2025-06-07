using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public class Withdrawal : BaseEvent
{
    public decimal Amount { get; set; }

    public Withdrawal()
    {
        base.EventName = nameof(Withdrawal);
    }
}
