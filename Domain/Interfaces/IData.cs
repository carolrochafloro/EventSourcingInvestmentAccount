using Domain.Entities;
using Domain.Events;

namespace Domain.Interfaces;

public interface IData
{
    void SaveEvent(BaseEvent evt);
    List<BaseEvent> GetAllEvents(string account);
    List<BaseEvent> GetEventsSince(string account, DateTime sinceDate);
    List<BaseEvent> GetEventsSinceUntil(string account, DateTime sinceDate, DateTime untilDate);
    BaseEvent GetEventById(Guid id);
    Snapshot GetSnapshot(DateTime? date, string account);
    void SaveSnapshot(Snapshot snapshot);
    void CreateAccount(Account account);
    Account GetAccount(string account);
}
