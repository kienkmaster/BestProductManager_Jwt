namespace BestProductManager.Api.Dtos.Account
{
    /// <summary>
    /// DTO mô tả thông tin một người dùng kèm trạng thái role phục vụ quản lý thành viên.
    /// </summary>
    public class UserWithRolesDto
    {
        /// <summary>
        /// Khóa chính của người dùng.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên đăng nhập (username) của người dùng.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Cờ cho biết người dùng có thuộc role Admin hay không.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
