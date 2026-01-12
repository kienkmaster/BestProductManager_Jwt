using BestProductManager.Dtos.Department;

namespace BestProductManager.Repositories
{
    /// <summary>
    /// Repository truy xuất dữ liệu phòng ban từ database.
    /// Thao tác thông qua stored procedure thuộc schema Department.
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Tạo mới phòng ban.
        /// Id được tính bằng cách lấy giá trị lớn nhất hiện tại từ stored [Department].[GetMaxDepartmentId]
        /// và cộng thêm 1 rồi lưu dưới dạng chuỗi.
        /// </summary>
        /// <param name="name">Tên phòng ban.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id phòng ban mới.</returns>
        Task<string> CreateDepartmentAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật tên phòng ban theo Id.
        /// </summary>
        /// <param name="id">Id phòng ban cần cập nhật.</param>
        /// <param name="name">Tên phòng ban mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại false.</returns>
        Task<bool> UpdateDepartmentAsync(
            string id,
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa phòng ban theo Id.
        /// </summary>
        /// <param name="id">Id phòng ban cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu xóa thành công, ngược lại false.</returns>
        Task<bool> DeleteDepartmentAsync(
            string id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy toàn bộ danh sách phòng ban từ stored [Department].[GetAllDepartments].
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách DepartmentDto.</returns>
        Task<IReadOnlyCollection<DepartmentDto>> GetAllDepartmentsAsync(
            CancellationToken cancellationToken = default);
    }
}
