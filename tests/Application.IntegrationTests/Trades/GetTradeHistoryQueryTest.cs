using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Trades.Commands.CreateTrade;
using Exchange.Application.Trades.Queries.GetTradeHistory;
using FluentAssertions;
using NUnit.Framework;

namespace Exchange.Application.IntegrationTests.Trades;
using static Testing;
public class GetTradeHistoryQueryTest : BaseTestFixture
{
    [Test]
    public async Task ShouldRetainTradeClient()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();

        var userId = await RunAsDefaultUserAsync();
        Assert.IsNotNull(userId);

        var command = new CreateTradeCommand
        {
            Amount = 5,
            From = "EUR",
            To = "GBP",
            Description = "Sample trade"
        };

        var commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);

        var getTradeHistoryResult = await SendAsync(new GetTradeHistoryQuery());
        Assert.IsNotNull(getTradeHistoryResult);
        Assert.AreEqual(2, getTradeHistoryResult.TotalCount);
        Assert.AreEqual(2, getTradeHistoryResult.Items.Count);
    }
}