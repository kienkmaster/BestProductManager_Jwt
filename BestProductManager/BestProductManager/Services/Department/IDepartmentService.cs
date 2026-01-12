using BestProductManager.Dtos.Department;

namespace BestProductManager.Services.Department
{
    /// <summary>
    /// Dịch vụ nghiệp vụ cho chức năng quản lý phòng ban.
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// Đăng ký phòng ban mới.
        /// </summary>
        /// <param name="request">Thông tin phòng ban cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id phòng ban mới được tạo.</returns>
        Task<string> CreateDepartmentAsync(
            CreateDepartmentRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật thông tin phòng ban.
        /// </summary>
        /// <param name="id">Id phòng ban cần cập nhật.</param>
        /// <param name="request">Thông tin tên phòng ban mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại false.</returns>
        Task<bool> UpdateDepartmentAsync(
            string id,
            UpdateDepartmentRequestDto request,
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
        /// Lấy toàn bộ danh sách phòng ban.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách DepartmentDto.</returns>
        Task<IReadOnlyCollection<DepartmentDto>> GetAllDepartmentsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm kiếm phòng ban theo Id và/hoặc tên phòng ban.
        /// Nếu nhập cả hai trường, kết quả phải thỏa đồng thời cả hai điều kiện.
        /// </summary>
        /// <param name="departmentId">Giá trị nhập từ "Id phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="name">Giá trị nhập từ "Tên phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách phòng ban thỏa điều kiện tìm kiếm.</returns>
        Task<IReadOnlyCollection<DepartmentDto>> SearchDepartmentsAsync(
            string? departmentId,
            string? name,
            CancellationToken cancellationToken = default);
    }
}
