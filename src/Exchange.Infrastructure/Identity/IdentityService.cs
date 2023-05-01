using AutoMapper.Internal;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Models;
using Exchange.Domain.Common;
using Exchange.Domain.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Exchange.Application.Common.Exceptions;
using Exchange.Infrastructure.Common;
using Exchange.Domain.Enums;
using Microsoft.Extensions.Options;
using Exchange.RabbitMQBus.Bus;
using Exchange.Domain.Events;

namespace Exchange.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IEventBus _eventBus;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
         SignInManager<ApplicationUser> signInManager,
         IOptions<JwtSettings> jwtSettings,
        IAuthorizationService authorizationService, IEventBus eventBus)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _eventBus = eventBus;
    }
    public async Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new ForbiddenAccessException($"No Accounts Registered with {request.Email}.");
        }
        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            throw new ForbiddenAccessException($"Invalid Credentials for '{request.Email}'.");
        }
        if (!user.EmailConfirmed)
        {
            throw new ForbiddenAccessException($"Account Not Confirmed for '{request.Email}'.");
        }
        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
        AuthenticationResponse response = new AuthenticationResponse
        {
            Id = user.Id,
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email,
            UserName = user.UserName
        };
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;

        string ipAddress = IpHelper.GetIpAddress();
        var refreshToken = GenerateRefreshToken(ipAddress);
        response.RefreshToken = refreshToken.Token;
        return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
    }
    public async Task<BaseResponse<string>> RegisterAsync(RegistrationRequest request, string origin)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            _eventBus.Publish(new RegisterClientEvent(new Guid()) { Email = request.Email, Name = request.FirstName + " " + request.LastName, UserAccountGuid = Guid.Parse(userWithSameUserName.Id), PhoneNumber = string.Empty });
            throw new ForbiddenAccessException($"Username '{request.UserName}' is already taken.");
        }
        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName
        };
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                var verificationUri = "";
                _eventBus.Publish(new RegisterClientEvent(new Guid()) { Email = request.Email, Name = request.FirstName + " " + request.LastName, UserAccountGuid = Guid.Parse(user.Id), PhoneNumber = string.Empty });
                return new BaseResponse<string>(user.Id, message: $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
            }
            else
            {
                throw new ForbiddenAccessException($"{result.Errors.ToList()[0].Description}");
            }
        }
        else
        {
            throw new ForbiddenAccessException($"Email {request.Email} is already registered.");
        }
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }
    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }
    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
    public async Task<Result> VerifyEmailAsync(VerifyAccount request)
    {
        var user = await _userManager.FindByIdAsync(request.UserUniqueIdentifier);
        if (user == null)
        {
            throw new ForbiddenAccessException($"No Accounts Registered with {request.UserUniqueIdentifier}.");
        }
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Result> UnVerifyEmailAsync(VerifyAccount request)
    {
        var user = await _userManager.FindByIdAsync(request.UserUniqueIdentifier);
        if (user == null)
        {
            throw new ForbiddenAccessException($"No Accounts Registered with {request.UserUniqueIdentifier}.");
        }
        user.EmailConfirmed = false;
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }
    #region static Method
    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        string ipAddress = IpHelper.GetIpAddress();

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        return new RefreshToken
        {
            Token = RandomTokenString(),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }
    private string RandomTokenString()
    {
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[40];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }


    #endregion
}
