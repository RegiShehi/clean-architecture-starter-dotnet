namespace Destination.Infrastructure.Identity.Tokens;

using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Authentication;
using Application.Features.Authentication.Login;
using Application.Features.Authentication.RefreshToken;
using Domain.Features.Authentication;
using Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using SharedKernel;

public class AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
    : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
        }

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InactiveUser);
        }

        return await GenerateTokenAndUpdateUserAsync(user);
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userPrincipal = GetClaimPrincipalFromExpiredToken(request.JwtToken);

        if (userPrincipal.IsFailure)
        {
            return Result.Failure<LoginResponse>(userPrincipal.Error);
        }

        var userEmail = userPrincipal.Value.GetEmail();

        var user = await userManager.FindByEmailAsync(userEmail);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InvalidToken);
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InactiveUser);
        }

        if (string.IsNullOrWhiteSpace(request.RefreshToken) ||
            string.IsNullOrWhiteSpace(user.RefreshToken) ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure<LoginResponse>(AuthErrors.InvalidToken);
        }

        var incomingHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

        if (!FixedTimeEquals(user.RefreshToken, incomingHash))
        {
            return Result.Failure<LoginResponse>(AuthErrors.InvalidToken);
        }

        return await GenerateTokenAndUpdateUserAsync(user);
    }

    [SuppressMessage(
        "Security",
        "CA5404:Do not disable token validation checks",
        Justification = "Disable for local development")]
    private Result<ClaimsPrincipal> GetClaimPrincipalFromExpiredToken(string expiredToken)
    {
        // TODO: Handle proper token validation in production
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal =
            tokenHandler.ValidateToken(expiredToken, validationParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ClaimsPrincipal>(AuthErrors.InvalidToken);
        }

        return principal;
    }

    private async Task<Result<LoginResponse>> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
    {
        var token = GenerateJwtToken(user);
        var (rawRefreshToken, tokenHash) = GenerateHashedRefreshToken();

        user.RefreshToken = tokenHash;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryTimeInDays);

        var updated = await userManager.UpdateAsync(user);

        if (!updated.Succeeded)
        {
            return Result.Failure<LoginResponse>(AuthErrors.PersistenceFailed);
        }

        return new LoginResponse
        {
            JwtToken = token,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
        };
    }

    private static (string RawToken, string TokenHash) GenerateHashedRefreshToken(int numBytes = 64)
    {
        var bytes = new byte[numBytes];
        RandomNumberGenerator.Fill(bytes);
        var rawToken = WebEncoders.Base64UrlEncode(bytes);

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        var tokenHash = Convert.ToHexString(hashBytes);

        return (rawToken, tokenHash);
    }

    private string GenerateJwtToken(ApplicationUser user) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetUserClaims(user));

    private string GenerateEncryptedToken(SigningCredentials credentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryTimeInMinutes),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var key = new SymmetricSecurityKey(secret);

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> GetUserClaims(ApplicationUser user) =>
    [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email ?? string.Empty),
        new(ClaimTypes.Name, user.FirstName ?? string.Empty),
        new(ClaimTypes.Surname, user.LastName ?? string.Empty),
        new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
    ];

    // Performs constant-time comparison to prevent timing attacks when verifying hashed tokens.
    private static bool FixedTimeEquals(string? a, string? b)
    {
        if (a is null || b is null || a.Length != b.Length)
        {
            return false;
        }

        var diff = 0;
        for (var i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }

        return diff == 0;
    }
}
