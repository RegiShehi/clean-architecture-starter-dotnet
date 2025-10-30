namespace CleanArchitecture.Application.Features.Identity.Roles.CreateRole;

using FluentValidation;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(c => c.CreateRoleRequest.Name)
        .NotEmpty()
        .WithMessage("Name is required.")
        .MinimumLength(3)
        .WithMessage("Name must be at least 3 characters long.");
}
