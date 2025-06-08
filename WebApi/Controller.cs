using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi;

public class Controller : ControllerBase
{
    private readonly IBusiness _business;

    public Controller(IBusiness business)
    {
        _business = business;
    }

    [HttpPost]
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

        } catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("/GetStateByDate")]
    public IActionResult GetStateByDate([FromQuery] DateOnly date, [FromQuery] string account)
    {
        try
        {
            Account accountState = _business.GetAccountStateByDate(account, date);
            return Ok(accountState);

        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
