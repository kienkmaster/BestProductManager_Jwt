namespace BestProductManager.Dtos.Admin
{
    /// <summary>
    /// DTO mô tả một role trong hệ thống dùng để bind combobox phân loại.
    /// </summary>
    public sealed class RoleDto
    {
        /// <summary>
        /// Khóa chính của role (Id trong bảng SecRole).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên role hiển thị (Name trong bảng SecRole).
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
