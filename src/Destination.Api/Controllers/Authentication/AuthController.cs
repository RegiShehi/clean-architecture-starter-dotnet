namespace Destination.Api.Controllers.Authentication;

using Application.Features.Authentication.Login;
using Application.Features.Authentication.RefreshToken;
using Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
public class AuthController : AppControllerBase
{
    // [HttpPost("login")]
    // public async Task<IResult> Login(
    //     [FromBody] LoginRequest request,
    //     [FromServices]
    //     ICommandHandler<LoginUserCommand, LoginResponse> handler,
    //     CancellationToken cancellationToken)
    // {
    //     var command = new LoginUserCommand(request);
    //
    //     var result = await handler.Handle(command, cancellationToken);
    //
    //     return result.Match(Results.Ok, CustomResults.Problem);
    // }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new LoginUserCommand(request), cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new RefreshTokenCommand(request), cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }
}
