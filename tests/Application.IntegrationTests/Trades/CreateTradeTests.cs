using Autofac.Core;
using Exchange.Application.Common.Exceptions;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Trades.Commands.CreateTrade;
using Exchange.Application.Trades.Queries.GetTradeHistory;
using FluentAssertions;
using NUnit.Framework;

namespace Exchange.Application.IntegrationTests.Trades;

using static Testing;

public class CreateTradeTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();
        var command = new CreateTradeCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
    [Test]
    public async Task NotSupportSymbols()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();
        var command = new CreateTradeCommand() { From = "test", To = "EUR", Amount = 10 };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
    [Test]
    public async Task ShouldCreateTradeCommand()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();

        var userId = await RunAsDefaultUserAsync();
        Assert.IsNotNull(userId);

        var command = new CreateTradeCommand
        {
            Amount = 5,
            From = "AED",
            To = "GBP",
            Description = "Sample trade"
        };

        var commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
    }
    [Test]
    public async Task ShouldNotBeMoreThan10TradePerHour()
    {
        var currencies = await SendAsync(new GetCurrencySymbolsQuery());
        currencies.Should().NotBeNull();

        var userId = await RunAsDefaultUserAsync();
        Assert.IsNotNull(userId);
        Thread.Sleep(2000);
        var command = new CreateTradeCommand
        {
            Amount = 5,
            From = "AED",
            To = "GBP",
            Description = "Sample trade",
        };

        var commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);
        commandResult = await SendAsync(command);
        Assert.IsNotNull(commandResult);
        Assert.IsTrue(commandResult.success);
        Thread.Sleep(2000);


        await FluentActions.Invoking(() =>
         SendAsync(command)).Should().ThrowAsync<ValidationException>();

    }
}
