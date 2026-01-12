using FluentValidation;
using BestProductManager.Api.Dtos.Account;

/// <summary>
/// Validation cho chức năng đăng ký User
/// </summary>
public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập.")
        ;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Vui lòng nhập Password.")
            .MaximumLength(20).WithMessage("Password không được vượt quá 20 ký tự, vui lòng nhập lại.")
            //.Matches("[a-z]").WithMessage("Password phải chứa ít nhất 1 chữ thường.")
            //.Matches("[A-Z]").WithMessage("Password phải chứa ít nhất 1 chữ hoa.")
            //.Matches("[0-9]").WithMessage("Password phải chứa ít nhất 1 chữ số.")
            //.Matches("[^a-zA-Z0-9]").WithMessage("Password phải chứa ít nhất 1 ký tự đặc biệt.")
            //.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{1,20}$")
            //    .WithMessage("Password phải gồm chữ thường, chữ hoa, chữ số và ký tự đặc biệt.")
            ;

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("ConfirmPassword không khớp với Password, vui lòng nhập lại.")
        ;
    }
}
