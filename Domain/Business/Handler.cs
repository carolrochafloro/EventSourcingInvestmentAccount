using Domain.Entities;
using Domain.Enum;
using Domain.Events;
using Domain.Interfaces;

namespace Domain.Business;
public class Handler : IHandler
{
    private readonly IData _data;
    private readonly IQueue _queue;

    public Handler(IData data, IQueue queue)
    {
        _data = data;
        _queue = queue;
    }

    public List<BaseEvent> GetEventsForAccount(string account)
    {
        return _data.GetAllEvents(account);
    }

    public Snapshot GetSnapshotByDate(DateTime date, string account)
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
            DateTime lastSnapshotDateTime = DateTime.SpecifyKind(
                lastSnapshot.Timestamp.Date,
                DateTimeKind.Utc
            );

            eventsToReplay = _data.GetEventsSince(account, lastSnapshotDateTime);
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
            Timestamp = DateTime.UtcNow, // ✅ Timestamp seguro para PostgreSQL
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

    public Account GetAccountStateByDate(string account, DateTime date)
    {
        Account currentAccount = _data.GetAccount(account);
        Snapshot lastSnapshot = _data.GetSnapshot(date, account);

        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow;

        List<BaseEvent> eventsSinceLastSnapshot = _data.GetEventsSinceUntil(account, from, to);

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
    public void RollbackEvent(Guid id)
    {
        var originalEvent = _data.GetEventById(id);

        var reversal = new ReversalEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            originalEvent.Account,
            originalEvent.Amount,
            originalEvent.Id
        );

        reversal.ReversedEvent = originalEvent.GetType().Name;

        _queue.Produce(reversal);
    }

    public BaseEvent GetEventById(Guid id)
    {
        return _data.GetEventById(id);
    }

    private decimal CalculateBalance(IEnumerable<BaseEvent> events, decimal startingBalance)
    {
        var orderedEvents = events.OrderBy(e => e.Timestamp);

        foreach (BaseEvent ev in orderedEvents)
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
                    startingBalance += reversal.ReversedEvent switch
                    {
                        nameof(CapitalContribution) => -reversal.Amount,
                        nameof(Withdrawal) => reversal.Amount,
                        _ => throw new InvalidOperationException($"Tipo de evento reversível desconhecido: {reversal.ReversedEvent}")
                    };
                    break;

                default:
                    throw new InvalidOperationException($"Evento desconhecido: {ev.GetType().Name}");
            }
        }

        return startingBalance;
    }
}
