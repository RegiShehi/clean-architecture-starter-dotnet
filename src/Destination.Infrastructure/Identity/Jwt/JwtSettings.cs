﻿namespace Destination.Infrastructure.Identity.Jwt;

public class JwtSettings
{
    public required string Key { get; set; }

    public int TokenExpiryTimeInMinutes { get; set; }

    public int RefreshTokenExpiryTimeInDays { get; set; }
}
