namespace Destination.Application.Features.Authentication.RefreshToken;

using Abstractions.Messaging;
using Login;

public sealed record RefreshTokenCommand(RefreshTokenRequest RefreshTokenRequest) : ICommand<LoginResponse>;
