using BestProductManager.Api.Entities;
using BestProductManager.Models;
using BestProductManager.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

using System.Text;

/// <summary>
/// Triển khai dịch vụ phát hành và quản lý access token, refresh token.
/// Sử dụng JwtOptions để đọc cấu hình và AspNetUserTokens để lưu refresh token.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private const string TokenLoginProvider = "BestProductManager";
    private const string RefreshTokenName = "RefreshToken";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Khởi tạo JwtTokenService với UserManager và JwtOptions.
    /// </summary>
    /// <param name="userManager">UserManager dùng để thao tác với người dùng và token.</param>
    /// <param name="jwtOptionsAccessor">Accessor cung cấp JwtOptions.</param>
    public JwtTokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptionsAccessor)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptionsAccessor.Value;
    }

    /// <summary>
    /// Phát hành access token JWT cho user với danh sách role tương ứng.
    /// </summary>
    /// <param name="user">Người dùng cần phát hành token.</param>
    /// <param name="roles">Danh sách role của người dùng (chỉ đọc, enumerable).</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>Chuỗi access token JWT.</returns>
    public Task<string> GenerateAccessTokenAsync(
        ApplicationUser user,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        // Khởi tạo danh sách claim cơ bản gắn với user.
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        // Gắn thêm các claim role tương ứng với quyền của user.
        foreach (var role in roles)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        // Tạo khóa ký từ cấu hình JwtOptions.
        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Tính thời điểm hết hạn của access token.
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

        // Tạo đối tượng JWT hoàn chỉnh.
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        // Ghi token ra chuỗi để trả về cho caller.
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwt);

        return Task.FromResult(token);
    }

    /// <summary>
    /// Phát hành refresh token mới cho user và lưu thông tin vào store Identity.
    /// Giá trị trả về là chuỗi cần lưu vào cookie refresh token.
    /// </summary>
    /// <param name="user">Người dùng cần phát hành refresh token.</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>Giá trị refresh token để ghi vào cookie.</returns>
    public async Task<string> GenerateAndStoreRefreshTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        // Sinh bytes ngẫu nhiên dùng làm giá trị refresh token.
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var tokenValue = Base64UrlEncoder.Encode(tokenBytes);

        // Tính hash của token (SHA256) để lưu vào DB thay vì lưu token nguyên bản.
        var tokenHash = ComputeSha256HashBase64Url(tokenValue);

        // Tính thời điểm hết hạn của refresh token.
        var expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

        // Đọc descriptor hiện tại (nếu có) để lấy previous token hash (dùng cho reuse detection).
        var existingJson = await _userManager.GetAuthenticationTokenAsync(
            user,
            TokenLoginProvider,
            RefreshTokenName);

        string? previousTokenHash = null;
        if (!string.IsNullOrWhiteSpace(existingJson))
        {
            try
            {
                var existing = JsonSerializer.Deserialize<RefreshTokenDescriptor>(existingJson);
                if (existing != null && !string.IsNullOrWhiteSpace(existing.TokenHash))
                {
                    previousTokenHash = existing.TokenHash;
                }
            }
            catch
            {
                // Nếu dữ liệu cũ không parse được, bỏ qua (vẫn tiếp tục tạo token mới).
            }
        }

        // Đóng gói thông tin refresh token vào descriptor nội bộ (lưu hash, không lưu token plain).
        var descriptor = new RefreshTokenDescriptor
        {
            TokenHash = tokenHash,
            PreviousTokenHash = previousTokenHash,
            ExpiresUtc = expires,
            CreatedUtc = DateTime.UtcNow,
            Revoked = false,
            ReplacedByTokenHash = null
        };

        // Serialize descriptor sang JSON để lưu trong AspNetUserTokens.
        var json = JsonSerializer.Serialize(descriptor);

        // Lưu JSON refresh token (chứa hash) vào store Identity dưới dạng authentication token.
        await _userManager.SetAuthenticationTokenAsync(
            user,
            TokenLoginProvider,
            RefreshTokenName,
            json);

        // Giá trị cookie gồm userId và tokenValue, phân tách bằng dấu '|'.
        // Lưu ý: cookie chứa token plain để client gửi lại; server chỉ lưu hash.
        var cookieValue = $"{user.Id}|{tokenValue}";

        return cookieValue;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của refresh token nhận từ cookie.
    /// Nếu hợp lệ, trả về user tương ứng.
    /// </summary>
    /// <param name="refreshToken">Giá trị refresh token từ cookie.</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>Bộ đôi (user, isValid) cho biết user và trạng thái hợp lệ.</returns>
    public async Task<(ApplicationUser? user, bool isValid)> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Kiểm tra rỗng.
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return (null, false);
        }

        // Cấu trúc cookie: userId|tokenValue.
        var parts = refreshToken.Split('|', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return (null, false);
        }

        var userId = parts[0];
        var tokenValue = parts[1];

        // Tìm user tương ứng với userId trong Identity.
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (null, false);
        }

        // Đọc JSON lưu refresh token (hash) từ AspNetUserTokens.
        var json = await _userManager.GetAuthenticationTokenAsync(
            user,
            TokenLoginProvider,
            RefreshTokenName);

        if (string.IsNullOrWhiteSpace(json))
        {
            return (null, false);
        }

        RefreshTokenDescriptor? descriptor;
        try
        {
            descriptor = JsonSerializer.Deserialize<RefreshTokenDescriptor>(json);
        }
        catch
        {
            // Dữ liệu không đúng định dạng mong đợi.
            return (null, false);
        }

        if (descriptor == null)
        {
            return (null, false);
        }

        // Nếu token đã bị thu hồi thì không hợp lệ.
        if (descriptor.Revoked)
        {
            return (null, false);
        }

        // Kiểm tra hết hạn.
        if (descriptor.ExpiresUtc < DateTime.UtcNow)
        {
            return (null, false);
        }

        // Tính hash của tokenValue gửi lên để so sánh với hash lưu trong DB.
        var incomingHash = ComputeSha256HashBase64Url(tokenValue);

        // Nếu khớp với token hiện tại => hợp lệ.
        if (string.Equals(descriptor.TokenHash, incomingHash, StringComparison.Ordinal))
        {
            return (user, true);
        }

        // Nếu khớp với previous token hash => phát hiện reuse (replay) của token cũ.
        if (!string.IsNullOrWhiteSpace(descriptor.PreviousTokenHash) &&
            string.Equals(descriptor.PreviousTokenHash, incomingHash, StringComparison.Ordinal))
        {
            // Hành vi đáng ngờ: token cũ được dùng lại sau khi đã rotate.
            // Hành động: thu hồi token hiện tại để bảo vệ tài khoản (revoke).
            await RevokeRefreshTokenAsync(user, cancellationToken);
            return (null, false);
        }

        // Không khớp => không hợp lệ.
        return (null, false);
    }

    /// <summary>
    /// Thu hồi (revoke) refresh token hiện tại của user.
    /// </summary>
    /// <param name="user">Người dùng cần thu hồi refresh token.</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    public async Task RevokeRefreshTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        // Xóa authentication token tương ứng với refresh token trong store Identity.
        await _userManager.RemoveAuthenticationTokenAsync(
            user,
            TokenLoginProvider,
            RefreshTokenName);
    }

    /// <summary>
    /// Phát hành cùng lúc access token và refresh token cho user.
    /// Dùng cho luồng đăng nhập ban đầu.
    /// </summary>
    /// <param name="user">Người dùng cần phát hành token.</param>
    /// <param name="roles">Danh sách role của người dùng.</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>Bộ đôi (accessToken, refreshToken).</returns>
    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(
        ApplicationUser user,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        // Phát hành access token dựa trên user và danh sách role.
        var accessToken = await GenerateAccessTokenAsync(user, roles, cancellationToken);

        // Phát hành và lưu refresh token, trả về giá trị chuỗi để ghi cookie.
        var refreshToken = await GenerateAndStoreRefreshTokenAsync(user, cancellationToken);

        return (accessToken, refreshToken);
    }

    /// <summary>
    /// Làm mới access token và refresh token dựa trên refresh token hiện có.
    /// Nếu refresh token hợp lệ, phát hành token mới và trả về user kèm danh sách role.
    /// </summary>
    /// <param name="refreshToken">Giá trị refresh token từ cookie.</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>
    /// Bộ giá trị (newAccessToken, newRefreshToken, user, roles).
    /// Nếu không hợp lệ, trả về chuỗi rỗng và user null.
    /// </returns>
    public async Task<(string newAccessToken, string newRefreshToken, ApplicationUser? user, IList<string> roles)> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Kiểm tra refresh token, nếu không hợp lệ thì trả về lỗi logic (user null, token rỗng).
        var (user, isValid) = await ValidateRefreshTokenAsync(refreshToken, cancellationToken);
        if (!isValid || user == null)
        {
            return (string.Empty, string.Empty, null, new List<string>());
        }

        // Đọc lại danh sách role hiện tại của user từ Identity.
        var roles = await _userManager.GetRolesAsync(user);

        // Phát hành access token mới.
        var newAccessToken = await GenerateAccessTokenAsync(user, roles, cancellationToken);

        // Phát hành refresh token mới và lưu vào store (cũ coi như bị thay thế).
        // Lưu ý: GenerateAndStoreRefreshTokenAsync sẽ đọc previous hash để hỗ trợ reuse detection.
        var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user, cancellationToken);

        return (newAccessToken, newRefreshToken, user, roles);
    }

    /// <summary>
    /// Kiểu dữ liệu nội bộ dùng để lưu trữ trạng thái refresh token trong AspNetUserTokens.
    /// Lưu hash của token hiện tại, hash token trước đó và các thông tin audit/phục vụ phát hiện reuse.
    /// </summary>
    private sealed class RefreshTokenDescriptor
    {
        // Lưu hash của token (SHA256 + Base64Url). Không lưu token plain.
        public string TokenHash { get; set; } = string.Empty;

        // Hash của token trước đó (nếu có) để phát hiện reuse khi rotation.
        public string? PreviousTokenHash { get; set; }

        // Nếu token bị thay thế thì có thể lưu hash token thay thế (không bắt buộc).
        public string? ReplacedByTokenHash { get; set; }

        // Thời điểm hết hạn của token.
        public DateTime ExpiresUtc { get; set; }

        // Thời điểm tạo token (dùng cho audit).
        public DateTime CreatedUtc { get; set; }

        // Cờ thu hồi token.
        public bool Revoked { get; set; }
    }

    /// <summary>
    /// Tính hash SHA256 cho chuỗi đầu vào và trả về kết quả dưới dạng Base64Url.
    /// Hàm này được dùng để tạo TokenHash và PreviousTokenHash trong RefreshTokenDescriptor.
    /// </summary>
    /// <param name="input">Chuỗi đầu vào cần băm.</param>
    /// <returns>Chuỗi hash đã được mã hóa theo chuẩn Base64Url.</returns>
    private static string ComputeSha256HashBase64Url(string input)
    {
        // Helper: tính SHA256 và trả về Base64Url string.
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Base64UrlEncoder.Encode(hash);
    }
}
