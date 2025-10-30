namespace Destination.Application.Features.Identity.Roles.GetRoleWithPermissions;

using Abstractions.Messaging;

public sealed record GetRoleWithPermissionsQuery(Guid RoleId) : IQuery<RoleDto>;
