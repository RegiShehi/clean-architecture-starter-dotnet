namespace CleanArchitecture.Application.Features.Identity.Users.GetUserPermissions;

using Abstractions.Messaging;

public sealed record GetUserPermissionsQuery(Guid UserId) : IQuery<List<string>>;
