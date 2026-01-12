using FluentValidation;
using BestProductManager.Api.Dtos.Products;
using BestProductManager.Api.Repositories;

namespace ProductApi.Validators.Product
{
    public class ProductSearchDtotValidator : AbstractValidator<ProductSearchDto>
    {
        private readonly IProductRepository _repo;

        public ProductSearchDtotValidator(IProductRepository repo)
        {

            _repo = repo;

            RuleFor(x => x.Keyword)
                .NotEmpty().WithMessage("Vui lòng nhập từ khóa tìm kiếm.")
                .MustAsync(ProductExists)
                .WithMessage("Sản phẩm không tồn tại, vui lòng nhập lại.")
            ;
        }

        /// <summary>
        /// ProductExists
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<bool> ProductExists(string keyword, CancellationToken token)
        {
            var products = await _repo.SearchProductsAsync(keyword, token);
            if (products == null)
            {
                return false;
            } else
            {
                return true;
            }

        }
    }
}
