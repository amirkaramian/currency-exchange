
using Exchange.Api;
using Exchange.Application.Common.Interfaces;
using Exchange.Domain.Common;
using Exchange.Domain.Entities;
using Exchange.Domain.Services.Authentication;
using Exchange.Infrastructure.Identity;
using Exchange.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Polly;
using Respawn;

namespace Exchange.Application.IntegrationTests;

[SetUpFixture]
public partial class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Respawner _checkpoint = null!;
    private static string? _currentUserId;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _factory = new CustomApiApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();

        _checkpoint = Respawner.CreateAsync(_configuration.GetConnectionString("DefaultConnection")!, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        }).GetAwaiter().GetResult();

    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }
   
    public static string? GetCurrentUserId()
    {
        return _currentUserId;
    }
    public static Guid GetCurrentUser()
    {
        using var scope = _scopeFactory.CreateScope();
        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        return currentUserService.AccountUserId();
    }
    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", "test", "user", Array.Empty<string>());
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", "Admin", "user", new[] { "Administrator" });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string firstName, string lastName, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var user = new RegistrationRequest { UserName = userName, Password = password, Email = userName, FirstName = firstName, LastName = lastName };

        var result = await identityService.RegisterAsync(user, "");
        Assert.NotNull(result);
        Assert.IsTrue(result.Success);
        if (result.Success)
        {
            _currentUserId = result.Data;

            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.ValidationErrors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task ResetState()
    {
        try
        {
            await _checkpoint.ResetAsync(_configuration.GetConnectionString("DefaultConnection")!);
        }
        catch (Exception)
        {
        }

        _currentUserId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }
    public static async Task<TEntity?> FindFirstAsync<TEntity>()
       where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  
        return await context.Set<TEntity>().FirstOrDefaultAsync();
    }
    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
