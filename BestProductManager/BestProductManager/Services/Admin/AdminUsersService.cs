using BestProductManager.Api.Entities;
using BestProductManager.Dtos.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BestProductManager.Api.Services.Admin
{
    /// <summary>
    /// Dịch vụ nghiệp vụ cho chức năng quản trị thành viên (Admin).
    /// Thao tác với UserManager và RoleManager để quản lý user và role.
    /// </summary>
    public class AdminUsersService : IAdminUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Khởi tạo AdminUsersService với UserManager và RoleManager.
        /// </summary>
        /// <param name="userManager">UserManager dùng để thao tác với bảng Users.</param>
        /// <param name="roleManager">RoleManager dùng để thao tác với bảng SecRole.</param>
        public AdminUsersService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Lấy danh sách tất cả thành viên trong hệ thống, kèm cờ IsAdmin và role chính hiện tại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách MemberSummaryDto phục vụ màn hình quản lý thành viên.</returns>
        public async Task<IReadOnlyCollection<MemberSummaryDto>> GetAllMembersAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Lấy danh sách user theo thứ tự tên đăng nhập.
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = new List<MemberSummaryDto>(users.Count);

            // Duyệt từng user để lấy role và tạo DTO tóm tắt.
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var isAdmin = roles.Contains("admin", StringComparer.OrdinalIgnoreCase);

                var primaryRoleName = roles.FirstOrDefault();
                string? primaryRoleId = null;

                // Xác định Id của role chính nếu tồn tại.
                if (!string.IsNullOrEmpty(primaryRoleName))
                {
                    var role = await _roleManager.FindByNameAsync(primaryRoleName).ConfigureAwait(false);
                    primaryRoleId = role?.Id;
                }

                var dto = new MemberSummaryDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    IsAdmin = isAdmin,
                    RoleId = primaryRoleId,
                    RoleName = primaryRoleName
                };

                result.Add(dto);
            }

            return result;
        }

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
        public async Task<(IdentityResult Result, string? TargetUserName)> ChangeMemberPasswordAsync(
            string userId,
            AdminChangePasswordRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Tìm user cần đổi mật khẩu.
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Không tìm thấy thành viên cần đổi mật khẩu."
                });

                return (error, null);
            }

            // Tạo reset token nội bộ và thực hiện đổi mật khẩu.
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var identityResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword)
                .ConfigureAwait(false);

            return (identityResult, user.UserName);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ role trong hệ thống dùng cho combobox phân loại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách RoleDto mô tả các role.</returns>
        public async Task<IReadOnlyCollection<RoleDto>> GetAllRolesAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Lấy danh sách role từ RoleManager.
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty
                })
                .ToList();

            return result;
        }

        /// <summary>
        /// Lấy thông tin phân loại (role) hiện tại của một thành viên.
        /// </summary>
        /// <param name="userId">Id của user cần tra cứu.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// Thông tin UserRoleInfoDto chứa userName và role hiện tại, hoặc null nếu không tìm thấy thành viên.
        /// </returns>
        public async Task<UserRoleInfoDto?> GetUserRoleAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Tìm user theo Id.
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            // Lấy danh sách role và xác định role chính.
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var primaryRoleName = roles.FirstOrDefault();
            string? primaryRoleId = null;

            if (!string.IsNullOrEmpty(primaryRoleName))
            {
                var role = await _roleManager.FindByNameAsync(primaryRoleName).ConfigureAwait(false);
                primaryRoleId = role?.Id;
            }

            var dto = new UserRoleInfoDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                RoleId = primaryRoleId,
                RoleName = primaryRoleName
            };

            return dto;
        }

        /// <summary>
        /// Cập nhật phân loại (role) chính cho một thành viên, do Admin thực hiện.
        /// Không cho phép thay đổi phân loại của thành viên thuộc nhóm quản lý (Admin).
        /// </summary>
        /// <param name="userId">Id của user cần cập nhật role.</param>
        /// <param name="dto">Thông tin role mới cần gán.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// IdentityResult và thông tin userName, roleName mới (để phục vụ ghi log).
        /// </returns>
        public async Task<(IdentityResult Result, string? TargetUserName, string? TargetRoleName)> UpdateUserRoleAsync(
            string userId,
            UpdateUserRoleRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Tìm user theo Id.
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Không tìm thấy thành viên cần cập nhật phân loại."
                });

                return (error, null, null);
            }

            // Tìm role mục tiêu theo RoleId.
            var targetRole = await _roleManager.FindByIdAsync(dto.RoleId).ConfigureAwait(false);
            if (targetRole == null)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = "Role mục tiêu không tồn tại."
                });

                return (error, user.UserName, null);
            }

            // Không cho phép thay đổi phân loại của thành viên thuộc role Admin.
            var currentRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var isTargetAdmin = currentRoles.Contains("admin", StringComparer.OrdinalIgnoreCase);
            if (isTargetAdmin)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotChangeAdminRole",
                    Description = "Không thể thay đổi phân loại của thành viên thuộc nhóm quản lý."
                });

                return (error, user.UserName, targetRole.Name);
            }

            // Xóa toàn bộ role hiện tại.
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles).ConfigureAwait(false);
            if (!removeResult.Succeeded)
            {
                return (removeResult, user.UserName, targetRole.Name);
            }

            // Gán role mới cho user.
            var addResult = await _userManager.AddToRoleAsync(user, targetRole.Name!).ConfigureAwait(false);
            if (!addResult.Succeeded)
            {
                return (addResult, user.UserName, targetRole.Name);
            }

            return (addResult, user.UserName, targetRole.Name);
        }
    }
}
