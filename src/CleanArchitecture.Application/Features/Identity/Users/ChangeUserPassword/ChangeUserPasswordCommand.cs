namespace CleanArchitecture.Application.Features.Identity.Users.ChangeUserPassword;

using Abstractions.Messaging;

public sealed record ChangeUserPasswordCommand(ChangePasswordRequest ChangePasswordRequest) : ICommand<Guid>;
