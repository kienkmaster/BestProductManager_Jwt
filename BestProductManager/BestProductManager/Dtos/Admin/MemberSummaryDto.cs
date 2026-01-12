namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO tóm tắt thông tin thành viên dùng cho màn hình quản lý thành viên.
    /// </summary>
    public sealed class MemberSummaryDto
    {
        /// <summary>
        /// Khóa chính của user (Id trong bảng Users).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên đăng nhập (UserName) của user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Cờ cho biết user có thuộc role Admin hay không.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Khóa chính của role chính hiện tại (Id trong bảng SecRole).
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Tên role chính hiện tại (Name trong bảng SecRole).
        /// </summary>
        public string? RoleName { get; set; }
    }
}
