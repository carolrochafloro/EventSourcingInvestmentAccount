using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public class CapitalContribution : BaseEvent
{
    public decimal Amount { get; set; }
    
    public CapitalContribution()
    {
        base.EventName = nameof(CapitalContribution);
    }
}
