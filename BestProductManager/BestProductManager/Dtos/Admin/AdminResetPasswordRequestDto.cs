using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO yêu cầu đổi mật khẩu của một người dùng do quản trị viên thực hiện.
    /// </summary>
    public class AdminResetPasswordRequestDto
    {
        /// <summary>
        /// Mật khẩu mới mà quản trị viên đặt cho người dùng.
        /// </summary>
        //[Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
