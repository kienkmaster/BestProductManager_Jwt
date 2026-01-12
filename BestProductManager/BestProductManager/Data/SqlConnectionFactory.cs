using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BestProductManager.Api.Data
{
    /// <summary>
    /// Triển khai ISqlConnectionFactory sử dụng SqlConnection cho SQL Server.
    /// Được dùng bởi các repository Dapper để tạo kết nối đến database.
    /// </summary>
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        /// <summary>
        /// Chuỗi kết nối đến SQL Server được lấy từ cấu hình ứng dụng.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo SqlConnectionFactory với connection string.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến SQL Server.</param>
        /// <exception cref="ArgumentNullException">Ném ra nếu connectionString null.</exception>
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Tạo một instance SqlConnection mới dựa trên connection string đã cấu hình.
        /// Kết nối được trả về ở trạng thái chưa mở.
        /// </summary>
        /// <returns>Đối tượng IDbConnection cho SQL Server.</returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
