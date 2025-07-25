﻿using Domain.Entities;
using Domain.Enum;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi;

[ApiController]
[Route("api/[controller]")]
public class Controller : ControllerBase
{
    private readonly IHandler _business;

    public Controller(IHandler business)
    {
        _business = business;
    }

    [HttpPost]
    [Route("/CreateAccount")]
    public IActionResult CreateAccount([FromQuery] string name)
    {
        try
        {
            Account newAccount = _business.CreateAccount(name);
            return Ok(newAccount);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("/CurrentState")]
    public IActionResult GetCurrentState([FromQuery] string account)
    {
        try
        {
            Account currentAccount = _business.GetCurrentState(account);
            return Ok(currentAccount);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("/AllEventsForAccount")]
    public IActionResult GetAllEventsForAccount([FromQuery] string account)
    {
        try
        {
            List<BaseEvent> evts = _business.GetEventsForAccount(account);
            return Ok(evts);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("/GetStateByDate")]
    public IActionResult GetStateByDate([FromQuery] DateTime date, [FromQuery] string account)
    {
        try
        {
            Account accountState = _business.GetAccountStateByDate(account, date);
            return Ok(accountState);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("/PublishTransaction")]
    public IActionResult PublishTransaction([FromQuery] decimal amount, [FromQuery] string account, [FromQuery] TransactionTypes transactionType)
    {
        try
        {
            _business.PublishEvent(amount, account, transactionType);
            return Ok("Transação registrada com sucesso.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("/RollbackEvent")]

    public IActionResult RollbackEvent(Guid id)
    {
        try
        {
            _business.RollbackEvent(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("/GetEventById")]
    public IActionResult GetEventById([FromQuery] Guid id)
    {
        try
        {
            var evt = _business.GetEventById(id);
            return Ok(evt);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
