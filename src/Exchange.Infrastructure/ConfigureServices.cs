using Exchange.Infrastructure.Identity;
using Exchange.Infrastructure.Persistence;
using Exchange.Infrastructure.Persistence.Interceptors;
using Exchange.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Interfaces;
using Exchange.Application.Services;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Exchange.Infrastructure;
using Exchange.Domain.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Exchange.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Exchange.Caching;
using FluentValidation;
using Exchange.Application.Trades.Commands.CreateTrade;
using Exchange.Application.Common.Models;
using Exchange.RabbitMQBus;
using Exchange.RabbitMQBus.Bus;
using Exchange.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.Configure<CasheSettings>(configuration.GetSection("CasheSettings"));
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ExchangeDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();
    

        services.AddHttpClient("FixerApi",
            options =>
            {
                options.BaseAddress = new Uri(configuration.GetValue<string>("FixerUrl"));
                options.DefaultRequestHeaders.Add("apikey", configuration.GetValue<string>("Apikey"));
            });
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IExchangeService, ExchangeService>();
      
        services.AddMemoryCache();
        services.AddRedis(configuration);
        services.AddIntegrationEventBus(configuration, configuration.GetValue<string>("IdentityExchangeQueue"));
        
        return services;
    }

}
