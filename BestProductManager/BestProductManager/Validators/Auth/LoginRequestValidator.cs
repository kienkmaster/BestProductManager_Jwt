using FluentValidation;
using BestProductManager.Api.Dtos.Account;

/// <summary>
/// Validation cho chức năng đăng nhập
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập.")
        ;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password chưa input, vui lòng nhập lại.")
        ;

    }
}
