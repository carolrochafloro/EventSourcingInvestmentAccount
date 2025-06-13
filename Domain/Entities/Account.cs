using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Account
{
    public required string AccountNumber { get; set; }
    public required string Name { get; set; }
    public decimal Balance { get; set; } = 0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; }
}
