using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.EntityFramework.Options;
using Exchange.Infrastructure.Identity;
using Exchange.Infrastructure.Persistence;
using Exchange.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure.Persistence;
public class IdentityDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    public IdentityDbContext(
      DbContextOptions<IdentityDbContext> options,
      IOptions<OperationalStoreOptions> operationalStoreOptions,
      AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
      IMediator mediator) : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
