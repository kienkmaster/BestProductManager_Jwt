using System;

namespace BestProductManager.Dtos.Department
{
    /// <summary>
    /// DTO dùng cho yêu cầu đăng ký phòng ban mới.
    /// </summary>
    public class CreateDepartmentRequestDto
    {
        /// <summary>
        /// Tên phòng ban cần đăng ký.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO mô tả thông tin phòng ban dùng cho phản hồi API.
    /// </summary>
    public class DepartmentDto
    {
        /// <summary>
        /// Khóa chính của phòng ban, lưu dưới dạng chuỗi số (NVARCHAR(450)).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên phòng ban.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
