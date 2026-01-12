using BestProductManager.Api.Dtos.Products;
using BestProductManager.Api.Entities;

namespace BestProductManager.Services.Products
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách sản phẩm dưới dạng DTO.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách ProductDto.</returns>
        Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm kiếm sản phẩm theo Id hoặc tên sản phẩm và trả về danh sách DTO tương ứng.
        /// </summary>
        /// <param name="keyword">Giá trị nhập từ người dùng (Id hoặc tên sản phẩm).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách ProductDto phù hợp với điều kiện tìm kiếm.</returns>
        Task<List<ProductDto>> SearchProductsAsync(string keyword, CancellationToken cancellationToken = default);

        /// <summary>
        /// CreateAsync
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
