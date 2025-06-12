using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Snapshot
{
    public Guid Id { get; set; } = new Guid();
    public decimal Balance { get; set; }
    public required string Account { get; set; }
    public DateTime Timestamp { get; set; }
}
