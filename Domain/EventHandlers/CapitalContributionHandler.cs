using Domain.Events;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.EventHandlers;
public class CapitalContributionHandler : ICapitalContributionHandler
{
    private readonly IData _data;
    private readonly ILogger<CapitalContributionHandler> _logger;

    public CapitalContributionHandler(IData data, ILogger<CapitalContributionHandler> logger)
    {
        _data = data;
        _logger = logger;
    }
    public void HandleEvent(CapitalContribution evt)
    {
        _logger.LogInformation($"Salvando o evento {evt.EventName} - timestamp {evt.Timestamp}");
        _data.SaveEvent(evt);
    }
}
