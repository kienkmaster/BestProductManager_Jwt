using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Api.Dtos.Account
{
    /// <summary>
    /// DTO dùng cho yêu cầu đăng nhập.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Tên đăng nhập.
        /// </summary>
        //[Required]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu.
        /// </summary>
        //[Required]
        public string Password { get; set; } = string.Empty;
    }
}
