﻿namespace CleanArchitecture.Application.Features.Identity.Users.ChangeUserPassword;

public class ChangePasswordRequest
{
    public required Guid UserId { get; set; }

    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }

    public required string ConfirmNewPassword { get; set; }
}
