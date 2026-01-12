namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO mô tả thông tin phân loại role hiện tại của một user.
    /// </summary>
    public sealed class UserRoleInfoDto
    {
        /// <summary>
        /// Khóa chính của user (Id trong bảng Users).
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Tên đăng nhập của user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Khóa chính của role hiện tại (Id trong bảng SecRole).
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Tên role hiện tại (Name trong bảng SecRole).
        /// </summary>
        public string? RoleName { get; set; }
    }
}
