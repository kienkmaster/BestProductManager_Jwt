using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO yêu cầu cập nhật role chính cho một user.
    /// </summary>
    public sealed class UpdateUserRoleRequestDto
    {
        /// <summary>
        /// Khóa chính của role mới cần gán cho user (Id trong bảng SecRole).
        /// </summary>
        [Required]
        public string RoleId { get; set; } = string.Empty;
    }
}
