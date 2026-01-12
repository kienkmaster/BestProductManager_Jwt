using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO dùng cho yêu cầu đổi mật khẩu của một thành viên khác, do Admin thực hiện.
    /// </summary>
    public sealed class AdminChangePasswordRequestDto
    {
        /// <summary>
        /// Mật khẩu mới Admin muốn thiết lập cho thành viên mục tiêu.
        /// </summary>
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
