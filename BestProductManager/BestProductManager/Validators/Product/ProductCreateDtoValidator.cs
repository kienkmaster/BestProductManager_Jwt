using FluentValidation;
using BestProductManager.Api.Dtos.Products;

namespace ProductApi.Validators.Product
{
    //public class ProductCreateDtoValidator : ProductBaseValidator<ProductCreateDto>
    //{
    //    public ProductCreateDtoValidator()
    //    {

    //        RuleFor(x => x.Name)
    //            .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm.")
    //        ;

    //        RuleFor(x => x.Description)
    //            .MaximumLength(500).WithMessage("Description không được vượt quá 500 ký tự")
    //        ;

    //        RuleFor(x => x.Price)
    //            .NotEmpty().WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
    //            .LessThanOrEqualTo(0).WithMessage("Giá thành sản phẩm không hợp lệ, vui lòng nhập lại.")
    //        ;

    //        RuleFor(x => x.Stock)
    //            .LessThan(0).WithMessage("Số lượng lưu kho không hợp lệ, vui lòng nhập lại.")
    //        ;
    //    }
    //}
}
