using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data;

public class Data : IData
{
    private readonly EventSourcingDbContext _context;

    public Data(EventSourcingDbContext context)
    {
        _context = context;
    }

    public void CreateAccount(Account account)
    {
        _context.Accounts.Add(account);
        _context.SaveChanges();
    }

    public Account GetAccount(string account)
    {
        return _context.Accounts.FirstOrDefault(a => a.AccountNumber == account);
    }

    public List<BaseEvent> GetAllEvents(string account)
    {
        return _context.Events
            .Where(e => e.Account == account)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    public BaseEvent GetEventById(Guid id)
    {
        return _context.Events.FirstOrDefault(e => e.Id == id);
    }

    public List<BaseEvent> GetEventsSince(string account, DateOnly date)
    {
        var since = date.ToDateTime(TimeOnly.MinValue);

        return _context.Events
            .Where(e => e.Account == account && e.Timestamp > since)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    public List<BaseEvent> GetEventsSinceUntil(string account, DateOnly sinceDate, DateOnly untilDate)
    {
        var since = sinceDate.ToDateTime(TimeOnly.MinValue);
        var until = untilDate.ToDateTime(TimeOnly.MaxValue);

        return _context.Events
            .Where(e => e.Account == account && e.Timestamp > since && e.Timestamp <= until)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    public Snapshot GetSnapshot(DateOnly? date, string account)
    {
        if (date == null)
        {
            return _context.Snapshots
                .Where(s => s.Account == account)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefault();
        }

        var until = date.Value.ToDateTime(TimeOnly.MaxValue);

        return _context.Snapshots
            .Where(s => s.Account == account && s.Timestamp <= until)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefault();
    }

    public void SaveEvent(BaseEvent evt)
    {
        _context.Events.Add(evt);
        _context.SaveChanges();
    }

    public void SaveSnapshot(Snapshot snapshot)
    {
        _context.Snapshots.Add(snapshot);
        _context.SaveChanges();
    }
}
