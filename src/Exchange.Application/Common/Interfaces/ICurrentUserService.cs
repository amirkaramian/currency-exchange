namespace Exchange.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid AccountUserId();
}
