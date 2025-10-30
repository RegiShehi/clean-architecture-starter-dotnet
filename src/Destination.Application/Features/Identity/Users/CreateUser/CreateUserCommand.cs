namespace Destination.Application.Features.Identity.Users.CreateUser;

using Abstractions.Messaging;

public sealed record CreateUserCommand(CreateUserRequest CreateUserRequest) : ICommand<Guid>;
