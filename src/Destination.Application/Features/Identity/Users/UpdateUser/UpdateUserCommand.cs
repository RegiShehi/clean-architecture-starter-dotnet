namespace Destination.Application.Features.Identity.Users.UpdateUser;

using Abstractions.Messaging;

public sealed record UpdateUserCommand(Guid UserId, UpdateUserRequest UpdateUserRequest) : ICommand<Guid>;
