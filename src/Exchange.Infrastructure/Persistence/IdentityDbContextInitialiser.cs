using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Domain.Enums;
using Exchange.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exchange.Infrastructure.Persistence;
public class IdentityDbContextInitialiser
{
    private readonly ILogger<IdentityDbContextInitialiser> _logger;
    private readonly IdentityDbContext _identityContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public IdentityDbContextInitialiser(ILogger<IdentityDbContextInitialiser> logger,
     IdentityDbContext identityContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _identityContext = identityContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task InitialiseAsync()
    {
        try
        {
            if (_identityContext.Database.IsSqlServer())
            {
                await _identityContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        foreach (var role in Enum.GetValues<Roles>())
        {
            var roleName = new IdentityRole(role.ToString());

            if (_roleManager.Roles.All(r => r.Name != roleName.Name))
            {
                await _roleManager.CreateAsync(roleName);
            }
        }
        var administratorRole = new IdentityRole(Roles.Admin.ToString());
        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost", FirstName = "admin", LastName = "user", EmailConfirmed = true };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "1qazXSW@");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }
    }
}
