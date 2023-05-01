using System.Reflection;
using Exchange.Application.Common.Behaviours;
using Exchange.Application.Common.Exceptions;
using Exchange.Application.IntegrationEvents;
using Exchange.Application.Interfaces;
using Exchange.Domain.Events;
using Exchange.RabbitMQBus.Bus;
using FluentValidation;
using MediatR;
using static System.Formats.Asn1.AsnWriter;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        });
        services.Scan(scan => scan
            .FromAssemblyOf<ISelfTransientLifetime>()
            .AddClasses(classes => classes.AssignableTo<ISelfTransientLifetime>())
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }
}
