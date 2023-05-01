using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Domain.Enums;
using Exchange.WebUI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Controllers;


public class CurrencyController : ApiControllerBase
{
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ILogger<CurrencyController> logger)
    {
        _logger = logger;
    }
    [HttpGet("/symbols")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<CurrencySymbols> Get()
    {
        return await Mediator.Send(new GetCurrencySymbolsQuery());
    }
    [HttpGet("/rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<CurrencyRate> GetRate([FromQuery] GetCurrencyRateQuery rateQuery)
    {
        return await Mediator.Send(rateQuery);
    }

}
