using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data;
public class Data : IData
{
    public void CreateAccount(Account account)
    {
        throw new NotImplementedException();
    }

    public Account GetAccount(string account)
    {
        throw new NotImplementedException();
    }

    public List<BaseEvent> GetAllEvents(string account)
    {
        throw new NotImplementedException();
    }

    public List<BaseEvent> GetEventsSince(string account, DateOnly date)
    {
        throw new NotImplementedException();
    }

    public Snapshot GetSnapshot(DateOnly? date, string account)
    {
        throw new NotImplementedException();
    }

    public void SaveEvent(BaseEvent evt)
    {
        throw new NotImplementedException();
    }

    public void SaveSnapshot(Snapshot snapshot)
    {
        throw new NotImplementedException();
    }
}
