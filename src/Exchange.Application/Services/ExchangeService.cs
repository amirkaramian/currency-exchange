using Exchange.Application.Common.Exceptions;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Models;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Interfaces;
using Exchange.Caching;
using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Exchange.Application.Services;
public class ExchangeService : IExchangeService
{
    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly ICacheManager _cacheManager;
    private readonly int _expriry;
    public ExchangeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache,
        ILogger<ExchangeService> logger, ICacheManager cacheManager, IOptions<CasheSettings> options)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("FixerApi");
        _memoryCache = memoryCache;
        _cacheManager = cacheManager;
        _expriry = options.Value.SymbolReadExpiryMinutes;
    }
    public async Task<CurrencyRate> GetRateAsync(GetCurrencyRateQuery request, CancellationToken cancellationToken)
    {
        try
        {
            CurrencyRate cacheValue = null;
            string key = $"{CacheKeys.EntryCurrencyRate}_{request.From}_{request.To}";
            var cacheData = await _cacheManager.GetAsync(key);
            if (string.IsNullOrEmpty(cacheData))
            {
                var response = await _httpClient.GetAsync($"/fixer/latest?symbols={request.To}&base={request.From}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                cacheValue = JsonConvert.DeserializeObject<CurrencyRate>(json);
                await _cacheManager.SetAsync(key, cacheValue, _expriry);
            }
            else
                cacheValue = JsonConvert.DeserializeObject<CurrencyRate>(cacheData);
            return cacheValue;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while getting Exchange rate request message");
            throw new RunTimeException(e.Message, e);
        }
    }

    public async Task<CurrencySymbols> GetSymbolsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_memoryCache.TryGetValue(CacheKeys.EntrySymbol, out CurrencySymbols cacheValue))
            {

                var response = await _httpClient.GetAsync("/fixer/symbols");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

              return JsonConvert.DeserializeObject<CurrencySymbols>(json);
                
            }

            return cacheValue;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while getting Exchange symbol request message");
            throw new RunTimeException("An error occured while getting Exchange symbol request message", e);
        }
    }

}
