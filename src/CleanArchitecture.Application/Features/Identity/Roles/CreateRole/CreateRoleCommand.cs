namespace CleanArchitecture.Application.Features.Identity.Roles.CreateRole;

using Abstractions.Messaging;

public sealed record CreateRoleCommand(CreateRoleRequest CreateRoleRequest) : ICommand<string>;
