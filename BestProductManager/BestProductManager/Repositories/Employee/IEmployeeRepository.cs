using BestProductManager.Dtos.Employee;
using BestProductManager.Entities.Employee;

namespace BestProductManager.Repository.Employee
{
    /// <summary>
    /// Repository thao tác dữ liệu nhân viên sử dụng Dapper và stored procedure.
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Lấy Id lớn nhất hiện tại của nhân viên từ stored [Employee].[GetMaxEmployeeId].
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Giá trị Id lớn nhất, 0 nếu chưa có record.</returns>
        Task<int> GetMaxEmployeeIdAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Đăng ký nhân viên mới thông qua stored [Employee].[CreateEmployee].
        /// </summary>
        /// <param name="employee">Thông tin nhân viên cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Số bản ghi bị ảnh hưởng.</returns>
        Task<int> CreateEmployeeAsync(EmployeeEntity employee, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật thông tin nhân viên qua stored [Employee].[UpdateEmployee].
        /// </summary>
        /// <param name="employee">Thông tin nhân viên sau cập nhật (Id không đổi).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, false nếu không tìm thấy bản ghi.</returns>
        Task<bool> UpdateEmployeeAsync(EmployeeEntity employee, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa nhân viên qua stored [Employee].[DeleteEmployee].
        /// </summary>
        /// <param name="id">Id nhân viên cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu xóa thành công, false nếu không tìm thấy bản ghi.</returns>
        Task<bool> DeleteEmployeeAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách tất cả nhân viên qua stored [Employee].[GetAllEmployees].
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách EmployeeDto phục vụ hiển thị.</returns>
        Task<IReadOnlyCollection<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);
    }
}
