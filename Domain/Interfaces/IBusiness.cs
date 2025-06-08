using Domain.Entities;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;
public interface IBusiness
{
    List<BaseEvent> GetEventsForAccount(string account);
    Snapshot GetSnapshotByDate(DateOnly date, string account);
    void CreateSnapshot(string account);
    Account GetCurrentState(string account);

}
