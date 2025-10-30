namespace Destination.Application.Features.Identity.Users.GetUserById;

using Abstractions.Messaging;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
