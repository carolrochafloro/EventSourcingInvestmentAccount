using Domain.Events;

namespace Domain.Interfaces;
public interface IQueue
{
    Task Produce(BaseEvent accountEvent);
    Task<BaseEvent> Consume();
}
