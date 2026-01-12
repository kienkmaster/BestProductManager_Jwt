using BestProductManager.Dtos.Admin;
using Microsoft.AspNetCore.Identity;

namespace BestProductManager.Api.Services.Admin
{
    /// <summary>
    /// Hợp đồng dịch vụ nghiệp vụ cho chức năng quản trị thành viên (Admin).
    /// Xử lý các nghiệp vụ: danh sách thành viên, đổi mật khẩu, quản lý role.
    /// </summary>
    public interface IAdminUsersService
    {
        /// <summary>
        /// Lấy danh sách tất cả thành viên trong hệ thống kèm cờ IsAdmin và role chính hiện tại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách MemberSummaryDto phục vụ màn hình quản lý thành viên.</returns>
        Task<IReadOnlyCollection<MemberSummaryDto>> GetAllMembersAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Đổi mật khẩu cho một thành viên xác định, do Admin thực hiện.
        /// Sử dụng cơ chế ResetPassword của ASP.NET Core Identity với reset token nội bộ.
        /// </summary>
        /// <param name="userId">Id của user cần đổi mật khẩu.</param>
        /// <param name="dto">Thông tin mật khẩu mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// Kết quả IdentityResult và tên người dùng mục tiêu (để phục vụ ghi log).
        /// Trường hợp không tìm thấy user: IdentityResult chứa lỗi với Code = "UserNotFound".
        /// </returns>
        Task<(IdentityResult Result, string? TargetUserName)> ChangeMemberPasswordAsync(
            string userId,
            AdminChangePasswordRequestDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách toàn bộ role trong hệ thống dùng cho combobox phân loại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách RoleDto mô tả các role.</returns>
        Task<IReadOnlyCollection<RoleDto>> GetAllRolesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy thông tin phân loại (role) hiện tại của một thành viên.
        /// </summary>
        /// <param name="userId">Id của user cần tra cứu.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// UserRoleInfoDto chứa thông tin userName và role hiện tại, hoặc null nếu không tìm thấy user.
        /// </returns>
        Task<UserRoleInfoDto?> GetUserRoleAsync(
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật phân loại (role) chính cho một thành viên, do Admin thực hiện.
        /// Quy ước: mỗi user chỉ có một role chính, khi cập nhật sẽ xóa toàn bộ role cũ và gán role mới.
        /// Không cho phép thay đổi phân loại của user thuộc nhóm quản lý (Admin).
        /// </summary>
        /// <param name="userId">Id của user cần cập nhật role.</param>
        /// <param name="dto">Thông tin role mới cần gán.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// IdentityResult và thông tin userName, roleName mới (để phục vụ ghi log).
        /// Các lỗi nghiệp vụ đặc thù:
        /// - "UserNotFound": không tìm thấy thành viên cần cập nhật.
        /// - "RoleNotFound": role mục tiêu không tồn tại.
        /// - "CannotChangeAdminRole": không được phép thay đổi phân loại của thành viên thuộc nhóm quản lý.
        /// </returns>
        Task<(IdentityResult Result, string? TargetUserName, string? TargetRoleName)> UpdateUserRoleAsync(
            string userId,
            UpdateUserRoleRequestDto dto,
            CancellationToken cancellationToken = default);
    }
}
