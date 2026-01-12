using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Department
{
    /// <summary>
    /// Dữ liệu yêu cầu đăng ký phòng ban mới.
    /// </summary>
    public class RegisterDepartmentRequestDto
    {
        /// <summary>
        /// Tên phòng ban cần đăng ký.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
