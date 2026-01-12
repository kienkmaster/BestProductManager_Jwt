using System.Security.Claims;
using BestProductManager.Api.Dtos.Account;
using BestProductManager.Api.Entities;
using BestProductManager.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace BestProductManager.Services.Account
{
    /// <summary>
    /// Hợp đồng dịch vụ nghiệp vụ cho chức năng tài khoản
    /// (đăng ký, đăng nhập, đăng xuất, đổi mật khẩu, tra cứu user hiện tại).
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Đăng ký người dùng mới.
        /// </summary>
        /// <param name="dto">Thông tin đăng ký tài khoản.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        Task<IdentityResult> RegisterAsync(
            RegisterUserDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra thông tin đăng nhập và trả về user hợp lệ kèm danh sách role.
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>
        /// Bộ giá trị gồm:
        /// - User hợp lệ hoặc null nếu đăng nhập thất bại.
        /// - Danh sách role của user.
        /// - Thông báo lỗi thân thiện nếu đăng nhập thất bại.
        /// </returns>
        Task<(ApplicationUser? User, IReadOnlyList<string> Roles, string? ErrorMessage)> LoginAsync(
            LoginRequestDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Xử lý nghiệp vụ đăng xuất trong mô hình xác thực hiện tại.
        /// </summary>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy thông tin người dùng hiện tại dựa trên ClaimsPrincipal.
        /// </summary>
        /// <param name="principal">Thông tin danh tính hiện tại của request.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        Task<CurrentUserResponseDto> GetCurrentUserAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Đổi mật khẩu cho người dùng hiện tại.
        /// </summary>
        /// <param name="userName">Tên đăng nhập của người dùng hiện tại.</param>
        /// <param name="dto">Thông tin mật khẩu cũ và mật khẩu mới.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        Task<IdentityResult> ChangePasswordAsync(
            string userName,
            ChangePasswordRequestDto dto,
            CancellationToken cancellationToken = default);
    }
}
