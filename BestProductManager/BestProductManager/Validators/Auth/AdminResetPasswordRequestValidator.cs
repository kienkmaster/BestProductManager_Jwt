using FluentValidation;
using BestProductManager.Dtos.Admin;

/// <summary>
/// Validation cho chức năng thay đổi mật khẩu
/// </summary>
public class AdminResetPasswordRequestValidator : AbstractValidator<AdminResetPasswordRequestDto>
{
    public AdminResetPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Vui lòng nhập Password.")
            .MaximumLength(20).WithMessage("Password không được vượt quá 20 ký tự, vui lòng nhập lại.")
        //.Matches("[a-z]").WithMessage("Password phải chứa ít nhất 1 chữ thường.")
        //.Matches("[A-Z]").WithMessage("Password phải chứa ít nhất 1 chữ hoa.")
        //.Matches("[0-9]").WithMessage("Password phải chứa ít nhất 1 chữ số.")
        //.Matches("[^a-zA-Z0-9]").WithMessage("Password phải chứa ít nhất 1 ký tự đặc biệt.")
        ;

    }
}
