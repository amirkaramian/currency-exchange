using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Trades.Commands.CreateTrade;
using FluentAssertions;
using NUnit.Framework;

namespace Exchange.Application.IntegrationTests.Currencies;
using static Testing;
public class GetCurrencySymbolsQueryTest: BaseTestFixture
{

    [Test]
    public async Task ShouldGetCurrencySymbolsQuery()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();
        currencies.Should().BeOfType<CurrencySymbols>();
    }
}
