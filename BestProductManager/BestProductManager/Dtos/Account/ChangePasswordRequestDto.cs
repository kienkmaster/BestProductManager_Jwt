using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Account
{
    /// <summary>
    /// DTO dùng cho yêu cầu đổi mật khẩu của chính người dùng đang đăng nhập.
    /// </summary>
    public sealed class ChangePasswordRequestDto
    {
        /// <summary>
        /// Mật khẩu hiện tại của người dùng.
        /// </summary>
        //[Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu mới người dùng muốn đặt.
        /// </summary>
        //[Required]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu xác nhận cho mật khẩu mới.
        /// </summary>
        //[Required]
        //[Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu mới không khớp.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
