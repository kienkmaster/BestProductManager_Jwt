using System.Data;

namespace BestProductManager.Api.Data
{
    /// <summary>
    /// Hợp đồng factory tạo kết nối cơ sở dữ liệu dùng cho Dapper.
    /// Cho phép khởi tạo IDbConnection đã cấu hình sẵn connection string của ứng dụng.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Tạo một kết nối cơ sở dữ liệu mới dựa trên connection string đã cấu hình.
        /// Kết nối được trả về ở trạng thái chưa mở, caller chịu trách nhiệm mở/đóng kết nối.
        /// </summary>
        /// <returns>Đối tượng kết nối cơ sở dữ liệu IDbConnection.</returns>
        IDbConnection CreateConnection();
    }
}
