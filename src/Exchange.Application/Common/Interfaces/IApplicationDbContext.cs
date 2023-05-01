using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<UserAccount> UserAccounts { get; }
    public DbSet<Currency> Currencies { get; }
    public DbSet<Tenant> Tenants { get; }
    public DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
