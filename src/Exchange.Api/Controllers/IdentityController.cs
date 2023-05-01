using Exchange.Api.Filters;
using Exchange.Application.Common.Interfaces;
using Exchange.Domain.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Exchange.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiExceptionFilter]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;

    private readonly IIdentityService _identityService;
    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
    {
        return Ok(await _identityService.AuthenticateAsync(request));
    }
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> RegisterAsync(RegistrationRequest request)
    {
        var origin = Request.Headers["origin"];
        return Ok(await _identityService.RegisterAsync(request, origin));
    }
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> VerifyEmailAsync(VerifyAccount request)
    {
        return Ok(await _identityService.VerifyEmailAsync(request));
    }
    [HttpPost("unverify")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> UnVerifyEmailAsync(VerifyAccount request)
    {
        return Ok(await _identityService.UnVerifyEmailAsync(request));
    }
}
