namespace CleanArchitecture.Application.Features.Authentication.Login;

using Abstractions.Messaging;

public sealed record LoginUserCommand(LoginRequest LoginRequest) : ICommand<LoginResponse>;
