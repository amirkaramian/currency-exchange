
using System;
using Exchange.Application.Common.Exceptions;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Models;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Trades.EventHandlers;
using Exchange.Caching;
using Exchange.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Exchange.Application.Trades.Commands.CreateTrade;
public class CreateTradeCommandValidator : AbstractValidator<CreateTradeCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public CreateTradeCommandValidator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RuleFor(x => x.From).NotNull()
            .NotEmpty().WithMessage("From Currency is required.");
        RuleFor(x => x.To).NotNull()
        .NotEmpty().WithMessage("To Currency is required.");
        RuleFor(x => x.Amount).NotNull()
            .GreaterThan(0).WithMessage("Amount at least greater than or equal to 0.");

        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        if (!memoryCache.TryGetValue(CacheKeys.EntrySymbol, out CurrencySymbols cacheValue))
            throw new RunTimeException("could not do operation");

        RuleFor(x => x.From).Must(y => cacheValue.CurrencyMap.Any(x => x.Code == y.ToUpper()))
            .WithMessage("From Currency is required.");

        RuleFor(x => x.To).Must(y => cacheValue.CurrencyMap.Any(x => x.Code == y.ToUpper()))
           .WithMessage("From Currency is required.");

        var cache = _serviceProvider.GetRequiredService<ICacheManager>();
        var currentUserService = _serviceProvider.GetRequiredService<ICurrentUserService>();
        var history = cache.GetAsync($"{CacheKeys.EntryTrade}_{currentUserService.AccountUserId()}").GetAwaiter().GetResult();
        if (!string.IsNullOrEmpty(history))
        {
            var tradeHistory = JsonConvert.DeserializeObject<TradeHistoryCommand>(history);
            var option = _serviceProvider.GetRequiredService<IOptions<CasheSettings>>();
            if (tradeHistory?.Count >= option.Value.TradeCountPerHour)
            {
                RuleFor(x => x.TradeCount).GreaterThan(0).WithMessage("Your're number of transaction is limit to 10 times in each hour.");
            }
        }

    }
}
