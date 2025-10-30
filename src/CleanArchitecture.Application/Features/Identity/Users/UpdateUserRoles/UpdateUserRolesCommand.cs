namespace CleanArchitecture.Application.Features.Identity.Users.UpdateUserRoles;

using Abstractions.Messaging;

public sealed record UpdateUserRolesCommand(Guid UserId, UpdateUserRolesRequest UserRolesRequest) : ICommand<Guid>;
