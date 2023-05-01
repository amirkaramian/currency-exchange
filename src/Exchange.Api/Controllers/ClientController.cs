using System.Net;
using Exchange.Application.Common.Models;
using Exchange.Application.Trades.Commands.CreateTrade;
using Exchange.Application.Trades.Queries.GetTradeHistory;
using Exchange.WebUI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Controllers;

public class ClientController : ApiControllerBase
{
    private readonly ILogger<ClientController> _logger;
    
    public ClientController(ILogger<ClientController> logger)
    {
        _logger = logger;
    }
    [HttpPost("/trade")]
    [ProducesResponseType(typeof(CreateTradeCommandResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<CreateTradeCommandResult> MakeTrade([FromBody] CreateTradeCommand  createTrade)
    {

        return await Mediator.Send(createTrade);
    }
    [HttpGet("/history")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<PaginatedList<GetTradeHistoryResult>> GetAllTrades([FromQuery] GetTradeHistoryQuery createTrade)
    {

        return await Mediator.Send(createTrade);
    }
}
