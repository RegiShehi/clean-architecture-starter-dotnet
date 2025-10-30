namespace CleanArchitecture.Api.Controllers;

using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class AppControllerBase : ControllerBase
{
    private ISender? _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
