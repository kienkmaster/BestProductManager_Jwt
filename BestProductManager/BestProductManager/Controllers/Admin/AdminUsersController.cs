using BestProductManager.Api.Services.Admin;
using BestProductManager.Dtos.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestProductManager.Api.Controllers.Admin
{
    /// <summary>
    /// API quản trị danh sách thành viên, chỉ cho phép user có role Admin truy cập.
    /// Cung cấp:
    /// - Danh sách thành viên (kèm cờ IsAdmin và role hiện tại).
    /// - Đổi mật khẩu cho thành viên khác.
    /// - Quản lý phân loại (role) thành viên.
    /// </summary>
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUsersService _adminUsersService;
        private readonly ILogger<AdminUsersController> _logger;

        /// <summary>
        /// Khởi tạo AdminUsersController với AdminUsersService và Logger.
        /// </summary>
        /// <param name="adminUsersService">Dịch vụ nghiệp vụ quản trị thành viên.</param>
        /// <param name="logger">Logger dùng để ghi log cho các hoạt động quản trị thành viên.</param>
        public AdminUsersController(
            IAdminUsersService adminUsersService,
            ILogger<AdminUsersController> logger)
        {
            _adminUsersService = adminUsersService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả thành viên trong hệ thống, kèm cờ cho biết user có thuộc role Admin hay không
        /// và role chính hiện tại của user.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách MemberSummaryDto phục vụ màn hình quản lý thành viên.</returns>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<MemberSummaryDto>>> GetAllMembers(
            CancellationToken cancellationToken)
        {
            // Gọi service để lấy danh sách member phục vụ màn hình quản trị.
            var result = await _adminUsersService.GetAllMembersAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Đổi mật khẩu cho một thành viên xác định, do Admin thực hiện.
        /// Sử dụng cơ chế ResetPassword của ASP.NET Core Identity với reset token nội bộ.
        /// </summary>
        /// <param name="userId">Id của user cần đổi mật khẩu.</param>
        /// <param name="dto">Thông tin mật khẩu mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả đổi mật khẩu thành công hoặc thông tin lỗi.</returns>
        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangeMemberPassword(
            [FromRoute] string userId,
            [FromBody] AdminChangePasswordRequestDto dto,
            CancellationToken cancellationToken)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đổi mật khẩu.
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu đổi mật khẩu không hợp lệ.",
                    errors = ModelState
                });
            }

            // Gọi service để thực hiện nghiệp vụ đổi mật khẩu cho thành viên.
            var (identityResult, targetUserName) = await _adminUsersService.ChangeMemberPasswordAsync(
                userId,
                dto,
                cancellationToken);

            // Xử lý trường hợp không tìm thấy thành viên cần đổi mật khẩu.
            var firstError = identityResult.Errors.FirstOrDefault();
            if (firstError != null &&
                string.Equals(firstError.Code, "UserNotFound", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new
                {
                    message = firstError.Description
                });
            }

            // Xử lý các lỗi đổi mật khẩu khác.
            if (!identityResult.Succeeded)
            {
                var errorDescription = firstError?.Description
                                       ?? "Đổi mật khẩu cho thành viên không thành công.";

                return BadRequest(new
                {
                    message = errorDescription,
                    errors = identityResult.Errors
                });
            }

            // Ghi log khi Admin đổi mật khẩu cho thành viên thành công.
            _logger.LogInformation(
                "Admin {AdminName} đã đổi mật khẩu cho user {UserName}.",
                User?.Identity?.Name,
                targetUserName);

            return Ok(new
            {
                message = "Đổi mật khẩu cho thành viên thành công."
            });
        }

        /// <summary>
        /// Lấy danh sách toàn bộ role trong hệ thống dùng cho combobox phân loại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách RoleDto mô tả các role.</returns>
        [HttpGet("roles")]
        public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetAllRoles(
            CancellationToken cancellationToken)
        {
            // Gọi service để lấy danh sách role phục vụ combobox phân loại.
            var result = await _adminUsersService.GetAllRolesAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin phân loại (role) hiện tại của một thành viên.
        /// </summary>
        /// <param name="userId">Id của user cần tra cứu.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Thông tin UserRoleInfoDto chứa userName và role hiện tại.</returns>
        [HttpGet("{userId}/role")]
        public async Task<ActionResult<UserRoleInfoDto>> GetUserRole(
            [FromRoute] string userId,
            CancellationToken cancellationToken)
        {
            // Gọi service để lấy thông tin role hiện tại của thành viên.
            var dto = await _adminUsersService.GetUserRoleAsync(userId, cancellationToken);

            if (dto == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thành viên."
                });
            }

            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật phân loại (role) chính cho một thành viên, do Admin thực hiện.
        /// Quy ước: mỗi user chỉ có một role chính, khi cập nhật sẽ xóa toàn bộ role cũ và gán role mới.
        /// Không cho phép thay đổi phân loại của user thuộc nhóm quản lý (Admin).
        /// </summary>
        /// <param name="userId">Id của user cần cập nhật role.</param>
        /// <param name="dto">Thông tin role mới cần gán.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả cập nhật phân loại thành công hoặc thông tin lỗi.</returns>
        [HttpPost("{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(
            [FromRoute] string userId,
            [FromBody] UpdateUserRoleRequestDto dto,
            CancellationToken cancellationToken)
        {
            // Kiểm tra tính hợp lệ của dữ liệu yêu cầu.
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu cập nhật phân loại không hợp lệ.",
                    errors = ModelState
                });
            }

            // Gọi service để thực hiện nghiệp vụ cập nhật role cho thành viên.
            var (identityResult, targetUserName, targetRoleName) = await _adminUsersService.UpdateUserRoleAsync(
                userId,
                dto,
                cancellationToken);

            var firstError = identityResult.Errors.FirstOrDefault();

            // Xử lý trường hợp không tìm thấy thành viên cần cập nhật phân loại.
            if (firstError != null &&
                string.Equals(firstError.Code, "UserNotFound", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new
                {
                    message = firstError.Description
                });
            }

            // Xử lý trường hợp role mục tiêu không tồn tại hoặc không được phép thay đổi role Admin.
            if (firstError != null &&
                (string.Equals(firstError.Code, "RoleNotFound", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(firstError.Code, "CannotChangeAdminRole", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new
                {
                    message = firstError.Description,
                    errors = identityResult.Errors
                });
            }

            // Xử lý các lỗi cập nhật role khác (xóa role cũ, gán role mới).
            if (!identityResult.Succeeded)
            {
                var errorDescription = firstError?.Description
                                       ?? "Cập nhật phân loại không thành công.";

                return BadRequest(new
                {
                    message = errorDescription,
                    errors = identityResult.Errors
                });
            }

            // Ghi log khi Admin cập nhật phân loại thành viên thành công.
            _logger.LogInformation(
                "Admin {AdminName} đã cập nhật phân loại của user {UserName} thành role {RoleName}.",
                User?.Identity?.Name,
                targetUserName,
                targetRoleName);

            return Ok(new
            {
                message = "Cập nhật phân loại thành công."
            });
        }
    }
}
