using BestProductManager.Api.Dtos.Account;
using BestProductManager.Api.Entities;
using BestProductManager.Dtos.Account;
using BestProductManager.Services.Account;
using BestProductManager.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestProductManager.Api.Controllers
{
    /// <summary>
    /// API quản lý tài khoản người dùng (đăng ký, đăng nhập, đổi mật khẩu, tra cứu user hiện tại) sử dụng JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Dịch vụ xử lý JWT (tạo access token, refresh token, làm mới phiên đăng nhập).
        /// </summary>
        private readonly IJwtTokenService _jwtTokenService;

        /// <summary>
        /// Tùy chọn cấu hình JWT (Issuer, Audience, CookieName, thời hạn token...).
        /// </summary>
        private readonly BestProductManager.Models.JwtOptions _jwtOptions;

        /// <summary>
        /// Dịch vụ quản lý người dùng (UserManager) của ASP.NET Core Identity.
        /// Dùng để tìm user hiện tại và thao tác với refresh token trong store Identity.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Khởi tạo AccountController với dịch vụ tài khoản, Logger, cấu hình JWT
        /// và dịch vụ xử lý token (access token + refresh token).
        /// </summary>
        /// <param name="accountService">Dịch vụ nghiệp vụ tài khoản sử dụng ASP.NET Core Identity.</param>
        /// <param name="logger">Logger dùng để ghi nhận log xử lý tài khoản.</param>
        /// <param name="configuration">Cấu hình ứng dụng dùng để đọc thông tin JWT (ví dụ CookieName mặc định).</param>
        /// <param name="jwtTokenService">Dịch vụ phát hành và làm mới JWT token.</param>
        /// <param name="jwtOptions">Tùy chọn cấu hình JWT được bind từ section "Jwt" trong appsettings.</param>
        /// <param name="userManager">Dịch vụ quản lý người dùng Identity, dùng để tra cứu và thao tác với ApplicationUser.</param>
        public AccountController(
            IAccountService accountService,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService,
            Microsoft.Extensions.Options.IOptions<BestProductManager.Models.JwtOptions> jwtOptions,
            UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _logger = logger;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _userManager = userManager;
        }

        /// <summary>
        /// Đăng ký người dùng mới với userName và password.
        /// </summary>
        /// <param name="dto">Thông tin đăng ký người dùng.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>Kết quả đăng ký thành công hoặc thông tin lỗi.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserDto dto,
            CancellationToken cancellationToken)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào.
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu đăng ký không hợp lệ.",
                    errors = ModelState
                });
            }

            // Gọi service để xử lý nghiệp vụ đăng ký tài khoản.
            var identityResult = await _accountService.RegisterAsync(dto, cancellationToken);

            // Xử lý trường hợp đăng ký không thành công và trả về chi tiết lỗi.
            if (!identityResult.Succeeded)
            {
                var errorDescription = identityResult.Errors.FirstOrDefault()?.Description
                                       ?? "Đăng ký tài khoản không thành công.";

                // Trả về HTTP 400 kèm danh sách lỗi chi tiết cho client.
                return BadRequest(new
                {
                    message = errorDescription,
                    errors = identityResult.Errors
                });
            }

            // Ghi log khi tạo mới tài khoản thành công.
            _logger.LogInformation("Tạo mới user {UserName} thành công.", dto.UserName);

            // Trả về HTTP 200 kèm thông báo đăng ký thành công cho client.
            return Ok(new
            {
                message = "Đăng ký tài khoản thành công."
            });
        }

        /// <summary>
        /// Đăng nhập hệ thống bằng userName và password.
        /// Sau khi đăng nhập thành công, API tạo access token (JWT) và refresh token,
        /// lưu trong HttpOnly Cookie, đồng thời trả về userName và roles.
        /// </summary>
        /// <param name="dto">Thông tin đăng nhập bao gồm userName và password.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>Kết quả đăng nhập thành công hoặc thông tin lỗi.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto dto,
            CancellationToken cancellationToken)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đăng nhập.
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu đăng nhập không hợp lệ.",
                    errors = ModelState
                });
            }

            // Gọi service để kiểm tra thông tin đăng nhập và lấy user kèm danh sách role.
            var (user, roles, errorMessage) = await _accountService.LoginAsync(dto, cancellationToken);

            // Trường hợp thông tin đăng nhập không hợp lệ, trả về 401 Unauthorized.
            if (user == null)
            {
                return Unauthorized(new
                {
                    message = errorMessage ?? "Tên đăng nhập hoặc mật khẩu không hợp lệ."
                });
            }

            // Gọi dịch vụ JWT để tạo access token và refresh token tương ứng với user + roles.
            var (accessToken, refreshToken) = await _jwtTokenService.GenerateTokensAsync(
                user,
                roles,
                cancellationToken);

            // Ghi cookie access token và refresh token vào response.
            WriteAuthCookies(accessToken, refreshToken);

            // Ghi log đăng nhập thành công.
            _logger.LogInformation("User {UserName} đăng nhập thành công.", dto.UserName);

            // Trả về thông tin cơ bản gồm userName và danh sách role cho frontend.
            return Ok(new
            {
                userName = user.UserName,
                roles
            });
        }

        /// <summary>
        /// Đăng xuất khỏi hệ thống đối với mô hình JWT + HttpOnly Cookie.
        /// Thực hiện gọi service nghiệp vụ, thu hồi refresh token và xóa cookie chứa access token + refresh token.
        /// </summary>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>Kết quả đăng xuất.</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            // Gọi service để xử lý nghiệp vụ logout (nếu có) như cập nhật log, trạng thái đăng nhập...
            await _accountService.LogoutAsync(cancellationToken);

            try
            {
                // Lấy userName hiện tại từ ClaimsPrincipal.
                var userName = User?.Identity?.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    // Tìm user tương ứng trong Identity dựa trên userName.
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null)
                    {
                        // Thu hồi refresh token đang còn hiệu lực của user trên server.
                        await _jwtTokenService.RevokeRefreshTokenAsync(user, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                // Không chặn luồng logout nếu quá trình revoke gây lỗi; chỉ ghi log để theo dõi, điều tra.
                _logger.LogWarning(ex, "Lỗi khi thu hồi refresh token trong Logout.");
            }

            // Xóa toàn bộ cookie xác thực (access token và refresh token) khỏi trình duyệt.
            DeleteAuthCookies();

            // Ghi log đăng xuất.
            _logger.LogInformation("User {UserName} đã đăng xuất.", User?.Identity?.Name);

            // Trả về HTTP 200 kèm thông báo đăng xuất thành công.
            return Ok(new
            {
                message = "Đăng xuất thành công."
            });
        }

        /// <summary>
        /// Làm mới access token dựa trên refresh token lưu trong HttpOnly Cookie.
        /// API này được frontend gọi khi access token hết hạn để duy trì phiên đăng nhập.
        /// </summary>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>
        /// HTTP 200 với userName và danh sách role nếu làm mới thành công,
        /// hoặc 401 nếu refresh token không hợp lệ / đã hết hạn.
        /// </returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            // Đọc tên cookie refresh token từ cấu hình JwtOptions hoặc dùng giá trị mặc định.
            var refreshCookieName = _jwtOptions.RefreshCookieName;
            if (string.IsNullOrWhiteSpace(refreshCookieName))
            {
                refreshCookieName = "BestProductManager.RefreshToken";
            }

            // Lấy refresh token từ HttpOnly Cookie của request.
            if (!Request.Cookies.TryGetValue(refreshCookieName, out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new
                {
                    message = "Refresh token không tồn tại hoặc không hợp lệ."
                });
            }

            // Gọi dịch vụ JWT để xác thực và làm mới access token + refresh token.
            var (newAccessToken, newRefreshToken, refreshedUser, refreshedRoles) =
                await _jwtTokenService.RefreshTokensAsync(refreshToken, cancellationToken);

            // Nếu không làm mới được (refresh token sai / hết hạn) thì xóa cookie và yêu cầu đăng nhập lại.
            if (string.IsNullOrWhiteSpace(newAccessToken) ||
                string.IsNullOrWhiteSpace(newRefreshToken) ||
                refreshedUser == null)
            {
                try
                {
                    // refreshToken có dạng "userId|tokenValue" theo thiết kế.
                    // Cố gắng truy ra user tương ứng để thu hồi refresh token phía server nếu có thể.
                    var parts = refreshToken.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        var userId = parts[0];
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            await _jwtTokenService.RevokeRefreshTokenAsync(user, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Không chặn luồng xử lý nếu revoke gặp lỗi; chỉ ghi log cảnh báo.
                    _logger.LogWarning(ex, "Lỗi khi cố gắng thu hồi refresh token sau khi refresh thất bại.");
                }

                // Xóa cookie access token + refresh token trên trình duyệt để buộc user đăng nhập lại.
                DeleteAuthCookies();

                return Unauthorized(new
                {
                    message = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại."
                });
            }

            // Ghi lại cookie access token và refresh token mới vào response.
            WriteAuthCookies(newAccessToken, newRefreshToken);

            // Ghi log làm mới token thành công.
            _logger.LogInformation(
                "User {UserName} làm mới access token thành công.",
                refreshedUser.UserName
            );

            // Trả về thông tin user và danh sách role hiện tại để client cập nhật trạng thái.
            return Ok(new
            {
                userName = refreshedUser.UserName,
                roles = refreshedRoles
            });
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện tại dựa trên JWT token trong HttpOnly Cookie.
        /// API này được Angular gọi khi khởi động ứng dụng để khôi phục trạng thái đăng nhập và role.
        /// </summary>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>Thông tin userName và danh sách role hiện tại.</returns>
        [HttpGet("me")]
        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<CurrentUserResponseDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            // Gọi service để đọc thông tin người dùng hiện tại từ ClaimsPrincipal.
            var response = await _accountService.GetCurrentUserAsync(HttpContext.User, cancellationToken);

            // Trả về HTTP 200 OK với thông tin user hiện tại hoặc rỗng nếu chưa đăng nhập.
            return Ok(response);
        }

        /// <summary>
        /// Đổi mật khẩu cho chính người dùng đang đăng nhập.
        /// Rule mật khẩu sử dụng rule giống với đăng ký người dùng (Identity password options).
        /// </summary>
        /// <param name="dto">Thông tin mật khẩu cũ, mật khẩu mới và xác nhận mật khẩu mới.</param>
        /// <param name="cancellationToken">Token hủy yêu cầu khi client ngắt kết nối.</param>
        /// <returns>Kết quả đổi mật khẩu thành công hoặc thông tin lỗi.</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequestDto dto,
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

            // Lấy tên người dùng hiện tại từ context.
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new
                {
                    message = "Không xác định được người dùng hiện tại."
                });
            }

            // Gọi service để thực hiện nghiệp vụ đổi mật khẩu.
            var identityResult = await _accountService.ChangePasswordAsync(userName, dto, cancellationToken);

            // Trường hợp không xác định được user hiện tại trong service, trả về 401 Unauthorized.
            var userNotFound = identityResult.Errors
                .Any(e => string.Equals(e.Code, "UserNotFound", StringComparison.OrdinalIgnoreCase));

            if (userNotFound)
            {
                return Unauthorized(new
                {
                    message = "Không xác định được người dùng hiện tại."
                });
            }

            // Xử lý trường hợp đổi mật khẩu không thành công và trả về chi tiết lỗi.
            if (!identityResult.Succeeded)
            {
                var errorDescription = identityResult.Errors.FirstOrDefault()?.Description
                                       ?? "Đổi mật khẩu không thành công.";

                return BadRequest(new
                {
                    message = errorDescription,
                    errors = identityResult.Errors
                });
            }

            // Ghi log đổi mật khẩu thành công.
            _logger.LogInformation("User {UserName} đổi mật khẩu thành công.", userName);

            // Trả về HTTP 200 kèm thông báo đổi mật khẩu thành công cho client.
            return Ok(new
            {
                message = "Đổi mật khẩu thành công."
            });
        }

        /// <summary>
        /// Ghi cookie access token và refresh token với Path phù hợp.
        /// - Access token: dùng cho toàn bộ API.
        /// - Refresh token: chỉ gửi kèm khi gọi endpoint refresh.
        /// </summary>
        /// <param name="accessToken">Chuỗi access token JWT.</param>
        /// <param name="refreshToken">Giá trị refresh token (userId|tokenValue) dùng để ghi cookie.</param>
        private void WriteAuthCookies(string accessToken, string refreshToken)
        {
            // Xác định tên cookie access token từ JwtOptions hoặc cấu hình Jwt:CookieName.
            var accessCookieName = string.IsNullOrWhiteSpace(_jwtOptions.CookieName)
                ? (_configuration["Jwt:CookieName"] ?? "BestProductManager.AuthToken")
                : _jwtOptions.CookieName;

            // Xác định tên cookie refresh token từ JwtOptions hoặc dùng tên mặc định.
            var refreshCookieName = string.IsNullOrWhiteSpace(_jwtOptions.RefreshCookieName)
                ? "BestProductManager.RefreshToken"
                : _jwtOptions.RefreshCookieName;

            // Xác định trạng thái HTTPS hiện tại để thiết lập thuộc tính Secure.
            var isHttps = string.Equals(Request.Scheme, "https", StringComparison.OrdinalIgnoreCase);

            // Path base khi host dưới virtual directory (ví dụ: /BestProductManager).
            var pathBase = Request.PathBase.HasValue
                ? Request.PathBase.Value
                : string.Empty;

            // Path cho access token: "/" hoặc "/BestProductManager/".
            var accessPath = string.IsNullOrEmpty(pathBase)
                ? "/"
                : pathBase + "/";

            // Path cho refresh token: "/api/Account/refresh"
            // hoặc "/BestProductManager/api/Account/refresh".
            var refreshPath = string.IsNullOrEmpty(pathBase)
                ? "/api/Account/refresh"
                : pathBase + "/api/Account/refresh";

            // Cookie cho access token: dùng cho toàn API, cho phép gửi kèm tất cả request trong ứng dụng.
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Path = accessPath,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes)
            };

            // Cookie cho refresh token: chỉ gửi kèm khi gọi endpoint refresh để giảm bề mặt tấn công.
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Strict,
                Path = refreshPath,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
            };

            // Ghi cookie access token.
            Response.Cookies.Append(accessCookieName, accessToken, accessCookieOptions);

            // Ghi cookie refresh token.
            Response.Cookies.Append(refreshCookieName, refreshToken, refreshCookieOptions);
        }

        /// <summary>
        /// Xóa cookie access token và refresh token hiện tại khỏi trình duyệt.
        /// Hàm này được dùng trong luồng Logout hoặc khi refresh token không còn hợp lệ.
        /// </summary>
        private void DeleteAuthCookies()
        {
            // Xác định tên cookie access token từ JwtOptions hoặc cấu hình Jwt:CookieName.
            var accessCookieName = _jwtOptions.CookieName;
            if (string.IsNullOrWhiteSpace(accessCookieName))
            {
                accessCookieName = _configuration["Jwt:CookieName"] ?? "BestCookieCompany.AuthToken";
            }

            // Xác định tên cookie refresh token từ JwtOptions hoặc dùng tên mặc định.
            var refreshCookieName = _jwtOptions.RefreshCookieName;
            if (string.IsNullOrWhiteSpace(refreshCookieName))
            {
                refreshCookieName = "BestProductManager.RefreshToken";
            }

            // Thiết lập tùy chọn chung khi xóa cookie (HttpOnly, Secure, SameSite, Path).
            var isHttps = string.Equals(Request.Scheme, "https", StringComparison.OrdinalIgnoreCase);

            var deleteOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            // Xóa cookie access token và refresh token bằng tên và tùy chọn đã thiết lập.
            Response.Cookies.Delete(accessCookieName, deleteOptions);
            Response.Cookies.Delete(refreshCookieName, deleteOptions);
        }
    }
}
