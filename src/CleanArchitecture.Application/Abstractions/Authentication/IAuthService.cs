namespace CleanArchitecture.Application.Abstractions.Authentication;

using Features.Authentication.Login;
using Features.Authentication.RefreshToken;
using SharedKernel;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);

    Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}
