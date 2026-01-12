using AutoMapper;
using BestProductManager.Api.Dtos.Products;
using BestProductManager.Api.Entities;
using BestProductManager.Api.Repositories;
using System.Threading;

namespace BestProductManager.Services.Products
{
    /// <summary>
    /// Dịch vụ nghiệp vụ cho chức năng quản lý sản phẩm.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Khởi tạo ProductService với repository và AutoMapper.
        /// </summary>
        /// <param name="productRepository">Repository sản phẩm.</param>
        /// <param name="mapper">Thành phần AutoMapper.</param>
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm và ánh xạ sang DTO.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách ProductDto.</returns>
        public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllProductsAsync(cancellationToken);
            return _mapper.Map<List<ProductDto>>(products);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo Id hoặc tên và ánh xạ kết quả sang DTO.
        /// </summary>
        /// <param name="keyword">Giá trị nhập từ người dùng (Id hoặc tên sản phẩm).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách ProductDto phù hợp với điều kiện tìm kiếm.</returns>
        public async Task<List<ProductDto>> SearchProductsAsync(string keyword, CancellationToken cancellationToken = default)
        {
            // Gọi repository để thực hiện truy vấn dữ liệu từ cơ sở dữ liệu.
            var products = await _productRepository.SearchProductsAsync(keyword, cancellationToken);

            // Ánh xạ danh sách entity sang DTO trước khi trả về cho controller.
            return _mapper.Map<List<ProductDto>>(products);
        }

        /// <summary>
        /// CreateAsync
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default) => _productRepository.CreateAsync(product, cancellationToken);


        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken = default) => _productRepository.UpdateAsync(id, product, cancellationToken);


        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) => _productRepository.DeleteAsync(id, cancellationToken);
    }
}
