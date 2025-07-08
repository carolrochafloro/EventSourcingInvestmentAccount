using Domain.Entities;
using Domain.Enum;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;
public interface IHandler
{
    List<BaseEvent> GetEventsForAccount(string account);
    Snapshot GetSnapshotByDate(DateTime date, string account);
    void CreateSnapshot(string account);
    Account GetCurrentState(string account);
    Account CreateAccount(string name);
    Account GetAccountStateByDate(string account, DateTime date);
    void PublishEvent(decimal amount, string account, TransactionTypes transactionType);
    void RollbackEvent(Guid id);
    BaseEvent GetEventById(Guid id);
}
