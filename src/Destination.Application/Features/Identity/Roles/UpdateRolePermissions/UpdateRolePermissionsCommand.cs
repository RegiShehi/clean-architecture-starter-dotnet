namespace Destination.Application.Features.Identity.Roles.UpdateRolePermissions;

using Abstractions.Messaging;

public sealed record UpdateRolePermissionsCommand(
    Guid RoleId,
    UpdateRolePermissionsRequest UpdateRolePermissionsRequest)
    : ICommand<Guid>;
