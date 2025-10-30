namespace CleanArchitecture.Application.Features.Authentication.RefreshToken;

using Abstractions.Messaging;
using Login;

public sealed record RefreshTokenCommand(RefreshTokenRequest RefreshTokenRequest) : ICommand<LoginResponse>;
