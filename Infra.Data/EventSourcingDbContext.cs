using Domain.Entities;
using Domain.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data;
public class EventSourcingDbContext : DbContext
{
    public EventSourcingDbContext(DbContextOptions<EventSourcingDbContext> options) : base(options) { }

    public DbSet<BaseEvent> Events => Set<BaseEvent>();
    public DbSet<Snapshot> Snapshots => Set<Snapshot>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseEvent>()
            .HasDiscriminator<string>("EventType")
            .HasValue<CapitalContribution>(nameof(CapitalContribution))
            .HasValue<Withdrawal>(nameof(Withdrawal))
            .HasValue<ReversalEvent>(nameof(ReversalEvent));

        modelBuilder.Entity<BaseEvent>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<BaseEvent>()
           .Property(e => e.Account)
           .IsRequired();

        modelBuilder.Entity<BaseEvent>()
            .Property(e => e.Amount)
            .HasColumnType("numeric(18,2)");

        modelBuilder.Entity<Snapshot>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Snapshot>()
            .Property(s => s.Account)
            .IsRequired();

        modelBuilder.Entity<Snapshot>()
            .Property(s => s.Balance)
            .HasColumnType("numeric(18,2)");

        modelBuilder.Entity<Account>()
            .HasKey(a => a.AccountNumber);

        base.OnModelCreating(modelBuilder);
    }
}
