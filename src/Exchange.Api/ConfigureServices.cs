using System.Configuration;
using Duende.IdentityServer.Models;
using Exchange.Api.Services;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Models;
using Exchange.Domain.Events;
using Exchange.Domain.Services.Authentication;
using Exchange.Infrastructure.Persistence;
using Exchange.RabbitMQBus.Bus;
using Microsoft.AspNetCore.Mvc;
//using NSwag;
//using NSwag.Generation.Processors.Security;
//using ZymLabs.NSwag.FluentValidation;

namespace Exchange.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
     

        services.AddScoped<ICurrentUserService, CurrentUserService>();
   
        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();


        //services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        //{
        //    var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
        //    var loggerFactory = provider.GetService<ILoggerFactory>();

        //    return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        //});

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        //services.AddOpenApiDocument((configure, serviceProvider) =>
        //{
        //    var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

        //    // Add the fluent validations schema processor
        //    configure.SchemaProcessors.Add(fluentValidationSchemaProcessor);

        //    configure.Title = "Exchange API";
        //    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
        //    {
        //        Type = OpenApiSecuritySchemeType.ApiKey,
        //        Name = "Authorization",
        //        In = OpenApiSecurityApiKeyLocation.Header,
        //        Description = "Type into the textbox: Bearer {your JWT token}."
        //    });

        //    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        //});


        return services;
    }
 
}
