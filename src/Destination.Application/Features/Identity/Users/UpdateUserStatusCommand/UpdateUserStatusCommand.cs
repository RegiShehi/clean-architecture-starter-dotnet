namespace Destination.Application.Features.Identity.Users.UpdateUserStatusCommand;

using Abstractions.Messaging;

public sealed record UpdateUserStatusCommand(Guid UserId, bool Activation) : ICommand<Guid>;
