using Exchange.Api;
using Exchange.Application.Common.Interfaces;
using Exchange.Domain.Services.Authentication;
using Exchange.Infrastructure.Identity;
using Exchange.Infrastructure.Persistence;
using Exchange.RabbitMQBus.Bus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;

namespace Exchange.Application.IntegrationTests;

using static Testing;

internal class CustomApiApplicationFactory : WebApplicationFactory<Program>
{
    private static Respawner _checkpoint = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services
                .Remove<ICurrentUserService>()
                .AddTransient(provider => Mock.Of<ICurrentUserService>(s => s.UserId == GetCurrentUserId()));

            

            services.AddTransient<IIdentityService, IdentityService>();

            _checkpoint = Respawner.CreateAsync(builder.Configuration.GetConnectionString("IdentityConnection")!, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            }).GetAwaiter().GetResult();
          
        });
    }
}
