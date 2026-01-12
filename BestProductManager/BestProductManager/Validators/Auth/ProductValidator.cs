using FluentValidation;
using BestProductManager.Api.Dtos.Products;

/// <summary>
/// Validation cho chức năng đăng ký User
/// </summary>
public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm.")
        ;

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
            .LessThanOrEqualTo(0).WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
        ;

        RuleFor(x => x.Price)
            .LessThan(0).WithMessage("Số lượng lưu kho không hợp lệ, vui lòng nhập lại.")
        ;

        //RuleSet("UpdateProductCheck", () =>
        //    {
        //        RuleFor(x => x.ProductName)
        //            .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm.")
        //        ;

        //        RuleFor(x => x.Price)
        //            .NotEmpty().WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
        //            .LessThanOrEqualTo(0).WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
        //        ;

        //        RuleFor(x => x.Price)
        //            .LessThan(0).WithMessage("Số lượng lưu kho không hợp lệ, vui lòng nhập lại.")
        //        ;
        //    }
        //);
    }
}
