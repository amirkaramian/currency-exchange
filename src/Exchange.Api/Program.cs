using Exchange.Infrastructure;
using Exchange.Api;
using Exchange.Infrastructure.Persistence;
using Exchange.Application.Interfaces;
using Exchange.RabbitMQBus.Bus;
using Exchange.Application.IntegrationEvents;
using Exchange.Domain.Events;
using MediatR;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;

namespace Exchange.Api;
public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddApplicationServices();
        builder.Services.AddIdentityService(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApiServices();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerService();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMigrationsEndPoint();
            // Initialise and seed databases
            using (var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                await initialiser.InitialiseAsync();
                var identityInitialiser = scope.ServiceProvider.GetRequiredService<IdentityDbContextInitialiser>();
                await identityInitialiser.InitialiseAsync();
                await identityInitialiser.SeedAsync();
            }
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        Task task = Task.Run(async () =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var exchangeService = scope.ServiceProvider.GetRequiredService<IMediator>();
                await exchangeService.Send(new GetCurrencySymbolsQuery());

                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                eventBus.Subscribe<RegisterClientEvent, RegisterClientEventHandler>();
            }
        });


        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();


        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}