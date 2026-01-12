namespace BestProductManager.Models
{
    /// <summary>
    /// Tập hợp các thiết lập cho việc phát hành và xác thực JWT trong hệ thống.
    /// Giá trị được bind từ section "Jwt" trong appsettings.json.
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Issuer dùng để ký access token.
        /// Giá trị tương ứng với cấu hình Jwt:Issuer.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audience hợp lệ của token.
        /// Giá trị tương ứng với cấu hình Jwt:Audience.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Khóa bí mật đối xứng dùng để ký token.
        /// Giá trị tương ứng với cấu hình Jwt:Key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Tên cookie dùng để lưu access token JWT.
        /// Giá trị tương ứng với cấu hình Jwt:CookieName.
        /// </summary>
        public string CookieName { get; set; } = "BestProductManager.AuthToken";

        /// <summary>
        /// Tên cookie dùng để lưu refresh token.
        /// Giá trị tương ứng với cấu hình Jwt:RefreshCookieName.
        /// </summary>
        public string RefreshCookieName { get; set; } = "BestProductManager.RefreshToken";

        /// <summary>
        /// Thời gian sống của access token tính theo phút.
        /// Giá trị tương ứng với cấu hình Jwt:AccessTokenMinutes.
        /// </summary>
        public int AccessTokenMinutes { get; set; } = 15;

        /// <summary>
        /// Thời gian sống của refresh token tính theo ngày.
        /// Giá trị tương ứng với cấu hình Jwt:RefreshTokenDays.
        /// </summary>
        public int RefreshTokenDays { get; set; } = 7;
    }
}
