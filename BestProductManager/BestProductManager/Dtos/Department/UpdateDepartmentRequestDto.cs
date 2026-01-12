using System;

namespace BestProductManager.Dtos.Department
{
    /// <summary>
    /// DTO dùng cho yêu cầu cập nhật thông tin phòng ban.
    /// </summary>
    public class UpdateDepartmentRequestDto
    {
        /// <summary>
        /// Tên phòng ban mới.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
