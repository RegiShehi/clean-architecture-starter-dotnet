namespace CleanArchitecture.Application.Features.Identity.Users.GetAllUsers;

using Abstractions.Messaging;

public sealed record GetAllUsersQuery : IQuery<List<UserDto>>;
