using Domain.Events;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.EventHandlers;
public class WithdrawalHandler : IWithdrawalHandler
{
    private readonly IData _data;
    private readonly ILogger<WithdrawalHandler> _logger;

    public WithdrawalHandler(IData data, ILogger<WithdrawalHandler> logger)
    {
        _data = data;
        _logger = logger;
    }

    public void HandleEvent(Withdrawal evt)
    {
        _logger.LogInformation($"Salvando o evento {evt.EventName} - timestamp {evt.Timestamp}");
        _data.SaveEvent(evt);
    }
}
