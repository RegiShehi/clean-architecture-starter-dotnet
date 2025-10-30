namespace Destination.Api.Controllers.Identity;

using Application.Abstractions.Messaging;
using Application.Features.Identity.Users;
using Application.Features.Identity.Users.CreateUser;
using Application.Features.Identity.Users.DeleteUser;
using Application.Features.Identity.Users.GetAllUsers;
using Application.Features.Identity.Users.GetUserById;
using Application.Features.Identity.Users.GetUserPermissions;
using Application.Features.Identity.Users.GetUserRoles;
using Application.Features.Identity.Users.UpdateUser;
using Application.Features.Identity.Users.UpdateUserRoles;
using Application.Features.Identity.Users.UpdateUserStatusCommand;
using Destination.Infrastructure.Identity.Authentication;
using Destination.Infrastructure.Identity.Constants;
using Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // GET /api/users
    [HttpGet]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.Users)]
    public async Task<IActionResult> GetUsersAsync(
        IQueryHandler<GetAllUsersQuery, List<UserDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    // GET /api/users/{id}
    [HttpGet("{userId:guid}", Name = nameof(GetUserByIdAsync))]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.Roles)]
    public async Task<IActionResult> GetUserByIdAsync(
        Guid userId,
        IQueryHandler<GetUserByIdQuery, UserDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    // GET /api/users/{userId}/permissions
    [HttpGet("{userId:guid}/permissions")]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.RoleClaims)]
    public async Task<IActionResult> GetUserPermissionsAsync(
        Guid userId,
        IQueryHandler<GetUserPermissionsQuery, List<string>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserPermissionsQuery(userId);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    // GET /api/users/{userId}/roles
    [HttpGet("{userId:guid}/roles")]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.UserRoles)]
    public async Task<IActionResult> GetUserRolesAsync(
        Guid userId,
        IQueryHandler<GetUserRolesQuery, List<UserRoleDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserRolesQuery(userId);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    // POST /api/users
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUserAsync([FromBody] CreateUserRequest request,
        ICommandHandler<CreateUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(
            userId => CreatedAtRoute(
                nameof(GetUserByIdAsync),
                new { userId },
                new { id = userId }
            ),
            CustomResults.Problem
        );
    }

    // PATCH /api/users/{userId}
    [HttpPatch("{userId:guid}")]
    [ShouldHavePermission(DestinationAction.Update, DestinationFeature.Users)]
    public async Task<IActionResult> UpdateUserDetailsAsync(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        ICommandHandler<UpdateUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(userId, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }

    // PATCH /api/users/{userId}/status
    [HttpPatch("{userId:guid}/status")]
    [ShouldHavePermission(DestinationAction.Update, DestinationFeature.Users)]
    public async Task<IActionResult> UpdateUserStatusAsync(
        Guid userId,
        [FromBody] bool isActive,
        ICommandHandler<UpdateUserStatusCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserStatusCommand(userId, isActive);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }

    // PUT /api/users/{userId}/roles
    [HttpPut("{userId:guid}/roles")]
    [ShouldHavePermission(DestinationAction.Update, DestinationFeature.UserRoles)]
    public async Task<IActionResult> UpdateUserRolesAsync(
        Guid userId,
        [FromBody] UpdateUserRolesRequest request,
        ICommandHandler<UpdateUserRolesCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserRolesCommand(userId, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }

    // DELETE /api/users/{userId}
    [HttpDelete("{userId:guid}")]
    [ShouldHavePermission(DestinationAction.Delete, DestinationFeature.Users)]
    public async Task<IActionResult> DeleteUserAsync(
        Guid userId,
        ICommandHandler<DeleteUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(userId);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }
}
