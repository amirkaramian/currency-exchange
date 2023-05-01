using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using FluentAssertions;
using NUnit.Framework;

namespace Exchange.Application.IntegrationTests.Currencies;

using static Testing;
public class GetCurrencyRateQueryTest : BaseTestFixture
{

    [Test]
    public async Task ShouldGetCurrencyRateQuery()
    {
        var rates = await SendAsync(new GetCurrencyRateQuery());
        rates.Should().NotBeNull();
        rates.Should().BeOfType<CurrencyRate>();
    }
}