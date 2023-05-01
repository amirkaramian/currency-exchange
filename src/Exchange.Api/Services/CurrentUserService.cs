using System.Security.Claims;

using Exchange.Application.Common.Interfaces;
using Exchange.Infrastructure.Persistence;

namespace Exchange.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static Guid _AccountUserId;
    private readonly IServiceProvider _serviceProvider;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;

    }
    public Guid AccountUserId()
    {
        if (Guid.Empty != _AccountUserId)
            return _AccountUserId;
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        _AccountUserId = context.UserAccounts.Single(x => x.UserAccountGuid == Guid.Parse(UserId)).Id;
        return _AccountUserId;
    }
    public string? UserId => _httpContextAccessor.HttpContext?.User?.Claims.First(x => x.Type == "uid").Value;// FindFirstValue(ClaimTypes.NameIdentifier);
}
