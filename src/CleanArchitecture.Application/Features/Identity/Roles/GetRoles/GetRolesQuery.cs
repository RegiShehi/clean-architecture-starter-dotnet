namespace CleanArchitecture.Application.Features.Identity.Roles.GetRoles;

using Abstractions.Messaging;

public sealed record GetRolesQuery : IQuery<List<RoleDto>>;
