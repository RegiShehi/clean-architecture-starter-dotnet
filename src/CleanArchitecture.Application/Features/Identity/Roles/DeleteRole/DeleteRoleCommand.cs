namespace CleanArchitecture.Application.Features.Identity.Roles.DeleteRole;

using Abstractions.Messaging;

public sealed record DeleteRoleCommand(Guid RoleId) : ICommand<string>;
