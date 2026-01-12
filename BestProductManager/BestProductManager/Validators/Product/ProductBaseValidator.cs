using FluentValidation;
using BestProductManager.Api.Dtos.Products;

namespace ProductApi.Validators.Product
{
    //public class ProductBaseValidator<T> : AbstractValidator<T> where T : ProductBaseDto
    //{
    //    public ProductBaseValidator()
    //    {
    //        RuleFor(x => x.Name)
    //            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

    //        RuleFor(x => x.Description)
    //            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

    //        RuleFor(x => x.TagIds)
    //            .NotNull()
    //            .WithMessage("TagIds is required");

    //        RuleForEach(x => x.TagIds)
    //            .GreaterThan(0)
    //            .WithMessage("TagIds must contain positive integers");
    //    }
    //}
}
