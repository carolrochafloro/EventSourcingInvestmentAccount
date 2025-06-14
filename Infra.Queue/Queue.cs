using Domain.Events;
using Domain.Interfaces;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Infra.Queue;
public class Queue : IQueue, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string QueueName = "EVENT.HANDLER.QUEUE";
    private Queue(IConnection connection)
    {
        _connection = connection;
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(queue: QueueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null)
            .GetAwaiter()
            .GetResult();
    }

    public static async Task<Queue> CreateAsync(string hostName)
    {
        var factory = new ConnectionFactory { HostName = hostName };
        var connection = await factory.CreateConnectionAsync();
        return new Queue(connection);
    }

    public Task<BaseEvent> Consume()
    {
        var tcs = new TaskCompletionSource<BaseEvent>();
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                using var doc = JsonDocument.Parse(json);
                var eventName = doc.RootElement.GetProperty("EventName").GetString();

                if (eventName is null)
                {
                    throw new InvalidOperationException("Não foi possível obter o nome do evento.");
                }

                BaseEvent evt = eventName switch
                {
                    nameof(CapitalContribution) => JsonSerializer.Deserialize<CapitalContribution>(json),
                    nameof(Withdrawal) => JsonSerializer.Deserialize<Withdrawal>(json),
                    nameof(ReversalEvent) => JsonSerializer.Deserialize<ReversalEvent>(json),
                    _ => throw new InvalidOperationException("Evento desconhecido")
                };

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                tcs.TrySetResult(evt);
            }
            catch (Exception ex) 
            {
                tcs.TrySetException(ex);
                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            }
        };

        _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        return tcs.Task;
    }

    public async Task Produce(BaseEvent accountEvent)
    {
        var json = JsonSerializer.Serialize(accountEvent);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueName, mandatory: false, body: body);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
}
