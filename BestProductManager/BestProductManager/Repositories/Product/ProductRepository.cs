using System.Data;
using Dapper;
using BestProductManager.Api.Data;
using BestProductManager.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using BestProductManager.Api.Dtos.Products;

namespace BestProductManager.Api.Repositories
{
    /// <summary>
    /// Triển khai repository truy xuất dữ liệu cho thực thể Product sử dụng Dapper.
    /// Thực hiện truy vấn thông qua stored procedure thuộc schema Product.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        /// <summary>
        /// Factory tạo kết nối SQL Server dùng cho Dapper.
        /// </summary>
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        /// <summary>
        /// Khởi tạo ProductRepository với SqlConnectionFactory.
        /// </summary>
        /// <param name="sqlConnectionFactory">Factory tạo kết nối cơ sở dữ liệu cho Dapper.</param>
        public ProductRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách sản phẩm từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách sản phẩm.</returns>
        public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((System.Data.Common.DbConnection)connection).OpenAsync(cancellationToken);

            // Gọi stored procedure Product.GetAllProducts để lấy danh sách sản phẩm.
            var command = new CommandDefinition(
                "[Product].[GetAllProducts]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var products = await connection.QueryAsync<ProductDto>(command);

            return products.ToList();
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo Id hoặc tên sản phẩm từ cơ sở dữ liệu.
        /// Quy tắc:
        /// - Nếu keyword là số và tìm thấy theo Id: trả về sản phẩm có Id đó.
        /// - Nếu không tìm thấy theo Id hoặc keyword không phải số: tìm theo tên (LIKE, tìm tương đối).
        /// </summary>
        /// <param name="keyword">Giá trị nhập từ người dùng (Id hoặc tên sản phẩm cần tìm).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách sản phẩm phù hợp với điều kiện tìm kiếm.</returns>
        public async Task<List<ProductDto>> SearchProductsAsync(string keyword, CancellationToken cancellationToken = default)
        {
            // Chuẩn hóa từ khóa tìm kiếm trước khi gọi stored procedure.
            var trimmedKeyword = keyword?.Trim() ?? string.Empty;

            // Nếu từ khóa rỗng thì không gọi database, trả về danh sách rỗng.
            if (string.IsNullOrEmpty(trimmedKeyword))
            {
                return new List<ProductDto>();
            }

            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((System.Data.Common.DbConnection)connection).OpenAsync(cancellationToken);

            // Gọi stored procedure Product.SearchProducts với tham số @Keyword.
            var command = new CommandDefinition(
                "[Product].[SearchProducts]",
                new { Keyword = trimmedKeyword },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var products = await connection.QueryAsync<ProductDto>(command);

            return products.ToList();
        }

        /// <summary>
        /// CreateAsync
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            using IDbConnection conn = _sqlConnectionFactory.CreateConnection();
            await ((System.Data.Common.DbConnection)conn).OpenAsync(cancellationToken);

            var param = new DynamicParameters();

            param.Add("ProductName", product.ProductName);
            param.Add("Price", product.Price);
            param.Add("Stock", product.Stock);
            param.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(
                "[Product].[CreateProduct]",
                param,
                commandType: CommandType.StoredProcedure
            );

            return param.Get<int>("NewId");
        }

        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken = default)
        {
            using IDbConnection conn = _sqlConnectionFactory.CreateConnection();
            await ((System.Data.Common.DbConnection)conn).OpenAsync(cancellationToken);

            var affected = await conn.ExecuteAsync(
                "[Product].[UpdateProduct]",
                new {
                    Id = id,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Stock = product.Stock
                },
                commandType: CommandType.StoredProcedure
            );

            return affected > 0;
        }

        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            using IDbConnection conn = _sqlConnectionFactory.CreateConnection();
            await ((System.Data.Common.DbConnection)conn).OpenAsync(cancellationToken);

            var affected = await conn.ExecuteAsync(
                "[Product].[DeleteProduct]",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return affected > 0;
        }
    }
}
