using System.Security.Claims;
using Exchange.Application.Common.Models;
using Exchange.Domain.Common;
using Exchange.Domain.Services.Authentication;

namespace Exchange.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
    Task<BaseResponse<string>> RegisterAsync(RegistrationRequest request, string origin);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<string?> GetUserNameAsync(string userId);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> VerifyEmailAsync(VerifyAccount request);
    Task<Result> UnVerifyEmailAsync(VerifyAccount request);
}
