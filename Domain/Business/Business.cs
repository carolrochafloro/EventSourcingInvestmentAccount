using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;

namespace Domain.Business;
public class Business : IBusiness
{
    private readonly IData _data;

    public Business(IData data)
    {
        _data = data;
    }

    public List<BaseEvent> GetEventsForAccount(string account)
    {
        return _data.GetAllEvents(account);
    }

    public Snapshot GetSnapshotByDate(DateOnly date, string account) 
    {
        return _data.GetSnapshot(date, account);
    }

    public void CreateSnapshot(string account)
    {
        Snapshot lastSnapshot = _data.GetSnapshot(null, account);
        DateOnly lastSnapshotDate = DateOnly.FromDateTime(lastSnapshot.Timestamp);
        List<BaseEvent> eventsSinceLastSnapshot = _data.GetEventsSince(account, lastSnapshotDate);

        Account currentAccount = _data.GetAccount(account);
        currentAccount = ProcessEvents(eventsSinceLastSnapshot, currentAccount);

        Snapshot snapshot = new Snapshot
        {
            Account = currentAccount.AccountNumber,
            Timestamp = DateTime.Now,
            Amount = currentAccount.Balance,
        };

        _data.SaveSnapshot(snapshot);
    }

    public Account GetCurrentState(string account) 
    {
        List<BaseEvent> evts = _data.GetAllEvents(account);
        Account currentAccount = _data.GetAccount(account);
        
        currentAccount = ProcessEvents(evts, currentAccount);

        return currentAccount;
    }

    public Account CreateAccount(string name)
    {
        Account newAccount = new Account
        {
            Name = name,
            AccountNumber = new Random().Next(100_000_000, 1_000_000_000).ToString(),
            Balance = 0
        };

        _data.CreateAccount(newAccount);
        return newAccount;
    }

    public Account GetAccountStateByDate(string account, DateOnly date)
    {
        Account currentAccount = _data.GetAccount(account);
        Snapshot lastSnapshot = _data.GetSnapshot(date, account);
        List<BaseEvent> eventsSinceLastSnapshot = _data.GetEventsSince(account, date);

        currentAccount.Balance = lastSnapshot.Amount;

        currentAccount = ProcessEvents(eventsSinceLastSnapshot, currentAccount);

        return currentAccount;
    }

    private Account ProcessEvents(IEnumerable<BaseEvent> events, Account account) 
    {
        foreach (BaseEvent ev in events) 
        {
           switch (ev)
            {
                case CapitalContribution cc:
                    account.Balance += cc.Amount;
                    break;
                case Withdrawal wd:
                    account.Balance -= wd.Amount;
                    break;
                default:
                    throw new InvalidOperationException($"Evento desconhecido: {ev.GetType().Name}");
            }
        }

        return account;
    }
}
