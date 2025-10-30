namespace Destination.Application.Features.Identity.Users.GetUserRoles;

using Abstractions.Messaging;

public sealed record GetUserRolesQuery(Guid UserId) : IQuery<List<UserRoleDto>>;
