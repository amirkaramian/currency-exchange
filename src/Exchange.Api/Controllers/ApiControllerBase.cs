using Exchange.Api.Filters;
using Exchange.Application.Common.Security;
using MediatR;
 
using Microsoft.AspNetCore.Mvc;


namespace Exchange.WebUI.Controllers;


[Authorize]
[ApiController]
[ApiExceptionFilter]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
