namespace Destination.Api.Controllers.Identity;

using Application.Abstractions.Messaging;
using Application.Features.Identity.Roles;
using Application.Features.Identity.Roles.CreateRole;
using Application.Features.Identity.Roles.DeleteRole;
using Application.Features.Identity.Roles.GetRoleById;
using Application.Features.Identity.Roles.GetRoles;
using Application.Features.Identity.Roles.GetRoleWithPermissions;
using Application.Features.Identity.Roles.UpdateRole;
using Application.Features.Identity.Roles.UpdateRolePermissions;
using Destination.Infrastructure.Identity.Authentication;
using Destination.Infrastructure.Identity.Constants;
using Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    // GET /api/roles
    [HttpGet]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.Roles)]
    public async Task<IActionResult> GetRolesAsync(
        IQueryHandler<GetRolesQuery, List<RoleDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    // GET /api/roles/{id}?include=permissions
    [HttpGet("{roleId:guid}", Name = nameof(GetRoleByIdAsync))]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.Roles)]
    public async Task<IActionResult> GetRoleByIdAsync(
        Guid roleId,
        [FromQuery]
        string? include,
        IQueryHandler<GetRoleByIdQuery, RoleDto> basicHandler,
        IQueryHandler<GetRoleWithPermissionsQuery, RoleDto> withPermissionsHandler,
        CancellationToken cancellationToken)
    {
        var wantsPermissions = string.Equals(include, "permissions", StringComparison.OrdinalIgnoreCase);

        if (wantsPermissions)
        {
            var query = new GetRoleWithPermissionsQuery(roleId);
            var result = await withPermissionsHandler.Handle(query, cancellationToken);

            return result.Match(value => Ok(value), CustomResults.Problem);
        }
        else
        {
            var query = new GetRoleByIdQuery(roleId);
            var result = await basicHandler.Handle(query, cancellationToken);

            return result.Match(value => Ok(value), CustomResults.Problem);
        }
    }

    // GET /api/roles/{id}/permissions
    [HttpGet("{roleId:guid}/permissions")]
    [ShouldHavePermission(DestinationAction.View, DestinationFeature.RoleClaims)]
    public async Task<IActionResult> GetPermissionsAsync(
        Guid roleId,
        IQueryHandler<GetRoleWithPermissionsQuery, RoleDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRoleWithPermissionsQuery(roleId);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(
            role => Ok(role.Permissions),
            CustomResults.Problem);
    }

    // POST /api/roles
    [HttpPost]
    [ShouldHavePermission(DestinationAction.Create, DestinationFeature.Roles)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest request,
        ICommandHandler<CreateRoleCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(
            roleId => CreatedAtRoute(
                nameof(GetRoleByIdAsync),
                new { roleId },
                new { id = roleId }
            ),
            CustomResults.Problem
        );
    }

    // PUT /api/roles/{id}
    [HttpPut("{roleId:guid}")]
    [ShouldHavePermission(DestinationAction.Update, DestinationFeature.Roles)]
    public async Task<IActionResult> UpdateRoleAsync(
        Guid roleId,
        [FromBody] UpdateRoleRequest request,
        ICommandHandler<UpdateRoleCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoleCommand(roleId, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }

    [HttpPut("{roleId:guid}/permissions")]
    [ShouldHavePermission(DestinationAction.Update, DestinationFeature.RoleClaims)]
    public async Task<IActionResult> UpdateRoleClaimsAsync(
        Guid roleId,
        [FromBody] UpdateRolePermissionsRequest request,
        ICommandHandler<UpdateRolePermissionsCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRolePermissionsCommand(roleId, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }

    [HttpDelete("{roleId:guid}")]
    [ShouldHavePermission(DestinationAction.Delete, DestinationFeature.Roles)]
    public async Task<IActionResult> DeleteRoleAsync(Guid roleId,
        ICommandHandler<DeleteRoleCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(roleId);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(_ => NoContent(), CustomResults.Problem);
    }
}
