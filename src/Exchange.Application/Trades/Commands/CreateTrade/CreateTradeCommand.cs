using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Interfaces;
using Exchange.Application.Common.Interfaces;
using MediatR;
using Newtonsoft.Json;
using Exchange.Domain.Events;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Exchange.Application.Common.Models;
using Exchange.Application.Trades.EventHandlers;
using Exchange.Caching;
using Exchange.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;

namespace Exchange.Application.Trades.Commands.CreateTrade;
public record CreateTradeCommand : IRequest<CreateTradeCommandResult>
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int TradeCount { get; set; }
}
public class CreateTradeCommandHandler : IRequestHandler<CreateTradeCommand, CreateTradeCommandResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IExchangeService _currencyService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _memoryCache;
    private readonly int _tradeCount;
    public CreateTradeCommandHandler(IApplicationDbContext context, IExchangeService exchangeService,
        ICurrentUserService currentUserService, IOptions<CasheSettings> options, IMemoryCache memoryCache)
    {
        _context = context;
        _currencyService = exchangeService;
        _currentUserService = currentUserService;
        _memoryCache = memoryCache;
        _tradeCount = options.Value.TradeCountPerHour;
    }
    public async Task<CreateTradeCommandResult> Handle(CreateTradeCommand command, CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue(CacheKeys.EntrySymbol, out CurrencySymbols cacheValue))
            throw new RunTimeException("could not do operation");
        var currencyRate = await _currencyService.GetRateAsync(new GetCurrencyRateQuery() { From = command.From, To = command.To }, cancellationToken);

        var entity = new Domain.Entities.Transaction()
        {
            Type = Domain.Enums.TransactionType.Exchange,
            Amount = command.Amount,
            Description = command.Description,
            CreatedBy = _currentUserService.UserId,
            SourceCurrencyId = cacheValue.CurrencyMap.Single(x => x.Code == command.From.ToUpper()).Id,
            UserAccountId = _currentUserService.AccountUserId(),
            DestinationCurrencyId = cacheValue.CurrencyMap.Single(x => x.Code == command.To.ToUpper()).Id,
            DestionationCurrency = command.To.ToUpper(),
            Rate = currencyRate.rates.First().Value,
            ExchangeAmountResult = currencyRate.rates.First().Value * command.Amount,
            DateRate = DateTime.FromFileTime(currencyRate.timestamp),
        };

        entity.AddDomainEvent(new TradeCreatedEvent(entity));
        await _context.Transactions.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new CreateTradeCommandResult()
        {
            date = DateTime.Now,
            info = new Info()
            {
                rate = currencyRate.rates.First().Value,
                timestamp = currencyRate.timestamp,
            },
            query = new Query()
            {
                amount = command.Amount,
                from = command.From,
                to = command.To,
            },
            result = entity.ExchangeAmountResult,
            success = true,
        };
    }


}
