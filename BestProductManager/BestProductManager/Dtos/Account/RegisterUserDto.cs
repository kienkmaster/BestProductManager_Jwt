using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Api.Dtos.Account
{
    /// <summary>
    /// DTO dùng cho yêu cầu đăng ký tài khoản.
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>
        /// Tên đăng nhập mong muốn.
        /// </summary>
        //[Required]
        //[StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu đăng nhập.
        /// </summary>
        //[Required]
        //[StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Xác nhận lại mật khẩu để tránh nhập sai.
        /// </summary>
        //[Required]
        //[Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
