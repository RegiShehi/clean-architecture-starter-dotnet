namespace Destination.Application.Features.Identity.Roles.GetRoleById;

using Abstractions.Messaging;

public sealed record GetRoleByIdQuery(Guid RoleId) : IQuery<RoleDto>;
