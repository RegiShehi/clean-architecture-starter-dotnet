namespace CleanArchitecture.Application.Features.Identity.Roles.UpdateRole;

using Abstractions.Messaging;

public sealed record UpdateRoleCommand(Guid RoleId, UpdateRoleRequest UpdateRoleRequest) : ICommand<Guid>;
