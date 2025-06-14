using Domain.Events;
using Domain.Interfaces;

namespace WorkerApp;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IQueue _queue;
    private readonly IBusiness _business;
    private readonly ICapitalContributionHandler _capitalContributionHandler;
    private readonly IWithdrawalHandler _withdrawalHandler;
    private readonly IData _data;

    public Worker(ILogger<Worker> logger, IQueue queue, IBusiness business, ICapitalContributionHandler capitalContributionHandler, IWithdrawalHandler withdrawalHandler, IData data)
    {
        _logger = logger;
        _queue = queue;
        _business = business;
        _capitalContributionHandler = capitalContributionHandler;
        _withdrawalHandler = withdrawalHandler;
        _data = data;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                BaseEvent evt = await _queue.Consume();

                _logger.LogInformation("Evento recebido: {EventName} - Conta: {Account}", evt.EventName, evt.Account);

                switch (evt)
                {
                    case CapitalContribution cc:
                        _capitalContributionHandler.HandleEvent(cc);
                        break;

                    case Withdrawal wd:
                        _withdrawalHandler.HandleEvent(wd);
                        break;

                    case ReversalEvent reversal:
                        _logger.LogInformation("Reversão recebida para o evento: {OriginalEventId}", reversal.OriginalEventId);
                        _data.SaveEvent(reversal);
                        break;

                    default:
                        _logger.LogWarning("Evento não tratado: {EventName}", evt.EventName);
                        break;
                }

                _business.CreateSnapshot(evt.Account);

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
