using Domain.Entities;
using Domain.Events;

namespace Domain.Interfaces;
public interface IData
{
    void SaveEvent(BaseEvent evt);
    List<BaseEvent> GetAllEvents(string account);
    List<BaseEvent> GetEventsSince(string account, DateOnly date);
    Snapshot GetSnapshot(DateOnly? date, string account);
    Account GetAccount(string account);
    void SaveSnapshot(Snapshot snapshot);
    void CreateAccount(Account account);
}
