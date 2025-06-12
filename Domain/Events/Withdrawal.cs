using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events;
public record Withdrawal(string Account, decimal Amount) : BaseEvent(Guid.NewGuid(), DateTime.UtcNow, Account, Amount);
