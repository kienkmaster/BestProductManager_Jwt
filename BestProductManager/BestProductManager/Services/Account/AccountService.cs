using System.Security.Claims;
using BestProductManager.Api.Dtos.Account;
using BestProductManager.Api.Entities;
using BestProductManager.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace BestProductManager.Services.Account
{
    /// <summary>
    /// Dịch vụ nghiệp vụ cho chức năng tài khoản sử dụng ASP.NET Core Identity.
    /// Xử lý các nghiệp vụ đăng ký, đăng nhập, đổi mật khẩu và đọc thông tin user hiện tại.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo AccountService với UserManager.
        /// </summary>
        /// <param name="userManager">Thành phần quản lý người dùng Identity.</param>
        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Đăng ký người dùng mới với tên đăng nhập và mật khẩu.
        /// </summary>
        /// <param name="dto">Thông tin đăng ký tài khoản.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        public async Task<IdentityResult> RegisterAsync(
            RegisterUserDto dto,
            CancellationToken cancellationToken = default)
        {
            // Kiểm tra yêu cầu hủy từ phía client.
            cancellationToken.ThrowIfCancellationRequested();

            // Khởi tạo thực thể người dùng mới từ DTO.
            var user = new ApplicationUser
            {
                UserName = dto.UserName
            };

            // Gọi Identity để tạo user với mật khẩu tương ứng.
            var result = await _userManager.CreateAsync(user, dto.Password);

            // Trả về kết quả để controller xử lý và phản hồi client.
            return result;
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập và trả về user hợp lệ kèm danh sách role.
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        public async Task<(ApplicationUser? User, IReadOnlyList<string> Roles, string? ErrorMessage)> LoginAsync(
            LoginRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            // Kiểm tra yêu cầu hủy từ phía client.
            cancellationToken.ThrowIfCancellationRequested();

            // Tìm user theo tên đăng nhập.
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                // Trả về thông báo lỗi thân thiện khi không tìm thấy user.
                return (null, Array.Empty<string>(), "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            }

            // Kiểm tra mật khẩu tương ứng với user đã tìm được.
            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                // Trả về thông báo lỗi thân thiện khi mật khẩu không đúng.
                return (null, Array.Empty<string>(), "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            }

            // Lấy danh sách role hiện tại của user.
            var roles = await _userManager.GetRolesAsync(user);

            // Trả về user hợp lệ kèm danh sách role để controller sinh JWT.
            return (user, roles.ToArray(), null);
        }

        /// <summary>
        /// Xử lý nghiệp vụ đăng xuất trong mô hình JWT + HttpOnly Cookie.
        /// </summary>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        public Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            // Kiểm tra yêu cầu hủy từ phía client.
            cancellationToken.ThrowIfCancellationRequested();

            // Mô hình JWT + HttpOnly Cookie không lưu trạng thái phiên trên server,
            // nên không cần thao tác bổ sung tại service khi logout.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện tại dựa trên ClaimsPrincipal.
        /// </summary>
        /// <param name="principal">Thông tin danh tính hiện tại của request.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        public Task<CurrentUserResponseDto> GetCurrentUserAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            // Kiểm tra yêu cầu hủy từ phía client.
            cancellationToken.ThrowIfCancellationRequested();

            // Kiểm tra trường hợp chưa xác thực hoặc không có danh tính hợp lệ.
            if (principal?.Identity is not { IsAuthenticated: true })
            {
                var emptyResponse = new CurrentUserResponseDto
                {
                    UserName = null,
                    Roles = Array.Empty<string>()
                };

                return Task.FromResult(emptyResponse);
            }

            // Lấy tên người dùng từ Identity.Name.
            var userName = principal.Identity?.Name;

            // Nếu chưa có, cố gắng lấy từ các claim Name hoặc NameIdentifier.
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = principal.FindFirstValue(ClaimTypes.Name)
                           ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // Nếu vẫn không xác định được tên người dùng, trả về response rỗng.
            if (string.IsNullOrWhiteSpace(userName))
            {
                var emptyResponse = new CurrentUserResponseDto
                {
                    UserName = null,
                    Roles = Array.Empty<string>()
                };

                return Task.FromResult(emptyResponse);
            }

            // Lấy danh sách role từ các claim Role, loại bỏ giá trị rỗng và trùng lặp.
            var roles = principal
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Khởi tạo DTO chứa thông tin người dùng hiện tại.
            var response = new CurrentUserResponseDto
            {
                UserName = userName,
                Roles = roles
            };

            return Task.FromResult(response);
        }

        /// <summary>
        /// Đổi mật khẩu cho người dùng hiện tại.
        /// </summary>
        /// <param name="userName">Tên đăng nhập của người dùng hiện tại.</param>
        /// <param name="dto">Thông tin mật khẩu cũ và mật khẩu mới.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        public async Task<IdentityResult> ChangePasswordAsync(
            string userName,
            ChangePasswordRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            // Kiểm tra yêu cầu hủy từ phía client.
            cancellationToken.ThrowIfCancellationRequested();

            // Tìm user theo tên đăng nhập.
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                // Trả về IdentityResult lỗi đặc thù khi không tìm thấy user.
                var error = IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Không xác định được người dùng hiện tại."
                });

                return error;
            }

            // Gọi Identity để thực hiện đổi mật khẩu với mật khẩu cũ và mới.
            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            // Trả về kết quả Identity cho controller xử lý và phản hồi client.
            return result;
        }
    }
}
