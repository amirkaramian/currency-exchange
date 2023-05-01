using Exchange.Application.Interfaces;
using Exchange.Application.Common.Mappings;
using Exchange.Application.Common.Models;
using MediatR;
using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Exchange.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Exchange.Application.Currencies.Queries.GetCurrencySymbols;
public record GetCurrencySymbolsQuery : IRequest<CurrencySymbols>;
public class GetWeatherForecastsQueryHandler : IRequestHandler<GetCurrencySymbolsQuery, CurrencySymbols>
{
    private readonly IExchangeService _currencyService;
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;
    public GetWeatherForecastsQueryHandler(IApplicationDbContext context, IMemoryCache memoryCache, IExchangeService currencyService)
    {
        _context = context;
        _currencyService = currencyService;
        _memoryCache = memoryCache;
    }


    public async Task<CurrencySymbols> Handle(GetCurrencySymbolsQuery request, CancellationToken cancellationToken)
    {
        var currencySymbols = await _currencyService.GetSymbolsAsync(cancellationToken);
        if (currencySymbols != null)
        {
            var currencies = await _context.Currencies.ToListAsync();
            var noExistedItems = currencySymbols.Symbols.Where(x => !currencies.Select(y => y.Code).Contains(x.Key)).ToList();
            if (noExistedItems.Any())
            {
                int order = 0;
                noExistedItems.ForEach(item =>
                {
                    var entity = new Currency()
                    {
                        Code = item.Key,
                        IsActive = true,
                        Name = item.Value,
                        Symbol = item.Key,
                        Order = order++
                    };
                    currencies.Add(entity);

                });
                await _context.Currencies.AddRangeAsync(currencies);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));
            if (currencySymbols.CurrencyMap == null || !currencySymbols.CurrencyMap.Any())
            {
                currencySymbols.CurrencyMap = new List<CurrencyMap>();
                currencies.ForEach(item =>
                    currencySymbols.CurrencyMap.Add(new CurrencyMap(item.Code.ToUpper(), item.Name, item.Id)));
            }
            else
            {
                Parallel.ForEach(noExistedItems, item =>
                {
                    var currency = currencies.Single(x => x.Code == item.Key);
                    currencySymbols.CurrencyMap.Add(new CurrencyMap(currency.Code.ToUpper(), currency.Name, currency.Id));
                });
            }


            _memoryCache.Set($"{CacheKeys.EntrySymbol}", currencySymbols, cacheEntryOptions);

        }

        return currencySymbols;
    }
}
