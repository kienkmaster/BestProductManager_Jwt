using BestProductManager.Dtos.Employee;

namespace BestProductManager.Services.Employee
{
    /// <summary>
    /// Dịch vụ nghiệp vụ cho chức năng quản lý nhân viên.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Đăng ký nhân viên mới và trả về Id vừa cấp.
        /// </summary>
        /// <param name="dto">Thông tin nhân viên cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id nhân viên mới.</returns>
        Task<string> RegisterAsync(
            CreateEmployeeRequestDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật thông tin nhân viên.
        /// </summary>
        /// <param name="id">Id nhân viên cần cập nhật.</param>
        /// <param name="dto">Thông tin cập nhật.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, false nếu không tìm thấy nhân viên.</returns>
        Task<bool> UpdateAsync(
            string id,
            UpdateEmployeeRequestDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa một nhân viên theo Id.
        /// </summary>
        /// <param name="id">Id nhân viên cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu xóa thành công, false nếu không tìm thấy nhân viên.</returns>
        Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách tất cả nhân viên.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách EmployeeDto.</returns>
        Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm kiếm nhân viên theo nhiều điều kiện.
        /// </summary>
        /// <param name="request">Điều kiện tìm kiếm.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách EmployeeDto thỏa điều kiện.</returns>
        Task<IReadOnlyCollection<EmployeeDto>> SearchAsync(
            SearchEmployeesRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
