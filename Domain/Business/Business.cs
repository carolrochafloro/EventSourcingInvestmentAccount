using Domain.Entities;
using Domain.Enum;
using Domain.Events;
using Domain.Interfaces;

namespace Domain.Business;
public class Business : IBusiness
{
    private readonly IData _data;
    private readonly IQueue _queue;

    public Business(IData data, IQueue queue)
    {
        _data = data;
        _queue = queue;
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
        Snapshot? lastSnapshot = _data.GetSnapshot(null, account);
        List<BaseEvent> eventsToReplay;
        decimal startingBalance = 0;

        if (lastSnapshot != null)
        {
            DateOnly lastSnapshotDate = DateOnly.FromDateTime(lastSnapshot.Timestamp);
            eventsToReplay = _data.GetEventsSince(account, lastSnapshotDate);
            startingBalance = lastSnapshot.Balance;
        }
        else
        {
            eventsToReplay = _data.GetAllEvents(account);
        }

        decimal balance = CalculateBalance(eventsToReplay, startingBalance);

        Snapshot snapshot = new Snapshot
        {
            Account = account,
            Timestamp = DateTime.UtcNow,
            Balance = balance
        };

        _data.SaveSnapshot(snapshot);
    }

    public Account GetCurrentState(string account) 
    {
        List<BaseEvent> evts = _data.GetAllEvents(account);
        Account currentAccount = _data.GetAccount(account);
        
        currentAccount.Balance = CalculateBalance(evts, currentAccount.Balance);

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
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        List<BaseEvent> eventsSinceLastSnapshot = _data.GetEventsSinceUntil(account, date, currentDate);

        currentAccount.Balance = CalculateBalance(eventsSinceLastSnapshot, lastSnapshot.Balance);

        return currentAccount;
    }

    public void PublishEvent(decimal amount, string account, TransactionTypes transactionType)
    {
        BaseEvent evt = transactionType switch
        {
            TransactionTypes.CapitalContribution => new CapitalContribution(account, amount),
            TransactionTypes.Withdrawal => new Withdrawal(account, amount),
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), $"Tipo de transação não suportado: {transactionType}")
        };

        _queue.Produce(evt);
    }

    public void RollbackEvent(BaseEvent originalEvent)
    {
        var reversal = new ReversalEvent(
        Guid.NewGuid(),
        DateTime.UtcNow,
        originalEvent.Account,
        originalEvent.Amount,
        originalEvent.Id,
        originalEvent.EventName
    );

        _queue.Produce(reversal);
    }

    public BaseEvent GetEventById(Guid id)
    {
        var evt = _data.GetEventById(id);
        return evt;
    }

    private decimal CalculateBalance(IEnumerable<BaseEvent> events, decimal startingBalance)
    {
        foreach (BaseEvent ev in events)
        {
            switch (ev)
            {
                case CapitalContribution cc:
                    startingBalance += cc.Amount;
                    break;

                case Withdrawal wd:
                    startingBalance -= wd.Amount;
                    break;

                case ReversalEvent reversal:
                    startingBalance += reversal.OriginalEventName switch
                    {
                        nameof(CapitalContribution) => -reversal.Amount,
                        nameof(Withdrawal) => reversal.Amount,
                        _ => throw new InvalidOperationException($"Tipo de evento reversível desconhecido: {reversal.OriginalEventName}")
                    };
                    break;

                default:
                    throw new InvalidOperationException($"Evento desconhecido: {ev.GetType().Name}");
            }
        }

        return startingBalance;
    }
}
