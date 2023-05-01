using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Currencies.Queries.GetCurrencyRate;
using Exchange.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Exchange.Application.IntegrationTests;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Testing;
public class CurrentUserAccountServiceTest : BaseTestFixture
{

    [Test]
    public async Task ShouldFindUserAccount()
    {
        var userId = await RunAsDefaultUserAsync();
        userId.Should().NotBeNull();
        var currentuser = GetCurrentUserId();
        currentuser.Should().NotBeNull();
        currentuser.ToString().Should().Be(userId.ToString());
    }
}