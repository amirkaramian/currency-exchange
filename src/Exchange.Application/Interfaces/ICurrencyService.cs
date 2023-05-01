using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Trades.Commands.CreateTrade;

namespace Exchange.Application.Interfaces;
public interface IExchangeService
{
    Task<CurrencyRate> GetRateAsync(GetCurrencyRateQuery request, CancellationToken cancellationToken);
    Task<CurrencySymbols> GetSymbolsAsync(CancellationToken cancellationToken);
}

 