namespace Destination.Application.Features.Identity.Users.DeleteUser;

using Abstractions.Messaging;

public sealed record DeleteUserCommand(Guid UserId) : ICommand<Guid>;
