using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Interfaces;
using Exchange.Application.Services;
using MediatR;

namespace Exchange.Application.Currencies.Queries.GetCurrencyRate;
public record GetCurrencyRateQuery : IRequest<CurrencyRate>
{
    public string From  { get; set; }
    public string To  { get; set; }
}
public class GetCurrencyRateQueryHandler : IRequestHandler<GetCurrencyRateQuery, CurrencyRate>
{
    private readonly IExchangeService _currencyService;
    public GetCurrencyRateQueryHandler(IExchangeService currencyService)
    {
        _currencyService = currencyService;
    }

    public async Task<CurrencyRate> Handle(GetCurrencyRateQuery request, CancellationToken cancellationToken)
    {
        return await _currencyService.GetRateAsync(request, cancellationToken);
    }
}
