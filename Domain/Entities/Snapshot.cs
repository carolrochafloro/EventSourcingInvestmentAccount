using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Snapshot
{
    public Guid Id { get; set; }
    public required string Account { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
}
