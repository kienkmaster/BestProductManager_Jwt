using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BestProductManager.Api.Entities;

namespace BestProductManager.Services.Auth
{
    /// <summary>
    /// Dịch vụ phát hành và quản lý access token và refresh token.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Phát hành access token JWT cho user với danh sách role tương ứng.
        /// </summary>
        /// <param name="user">Người dùng cần phát hành token.</param>
        /// <param name="roles">Danh sách role của người dùng (chỉ cần enumerable để duyệt).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Chuỗi access token JWT.</returns>
        Task<string> GenerateAccessTokenAsync(
            ApplicationUser user,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Phát hành refresh token mới cho user và lưu thông tin vào store Identity.
        /// Giá trị trả về là chuỗi cần lưu vào cookie refresh token.
        /// </summary>
        /// <param name="user">Người dùng cần phát hành refresh token.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Giá trị refresh token để ghi vào cookie.</returns>
        Task<string> GenerateAndStoreRefreshTokenAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra tính hợp lệ của refresh token nhận từ cookie.
        /// Nếu hợp lệ, trả về user tương ứng.
        /// </summary>
        /// <param name="refreshToken">Giá trị refresh token từ cookie.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Bộ đôi (user, isValid) cho biết user và trạng thái hợp lệ.</returns>
        Task<(ApplicationUser? user, bool isValid)> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Thu hồi (revoke) refresh token hiện tại của user.
        /// </summary>
        /// <param name="user">Người dùng cần thu hồi refresh token.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        Task RevokeRefreshTokenAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Phát hành cùng lúc access token và refresh token cho user.
        /// Dùng trong luồng đăng nhập ban đầu.
        /// </summary>
        /// <param name="user">Người dùng cần phát hành token.</param>
        /// <param name="roles">Danh sách role của người dùng (enumerable).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Bộ đôi (accessToken, refreshToken) để lưu cookie / sử dụng.</returns>
        Task<(string accessToken, string refreshToken)> GenerateTokensAsync(
            ApplicationUser user,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Làm mới access token và refresh token dựa trên refresh token nhận được.
        /// Nếu hợp lệ, trả về token mới và thông tin người dùng kèm danh sách role.
        /// </summary>
        /// <param name="refreshToken">Giá trị refresh token từ cookie.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>
        /// Bộ giá trị (newAccessToken, newRefreshToken, user, roles).
        /// Nếu refresh token không hợp lệ, trả về chuỗi rỗng và user null.
        /// </returns>
        Task<(string newAccessToken, string newRefreshToken, ApplicationUser? user, IList<string> roles)> RefreshTokensAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);
    }
}
