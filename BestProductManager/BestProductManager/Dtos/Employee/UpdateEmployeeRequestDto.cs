using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Dtos.Employee
{
    /// <summary>
    /// DTO yêu cầu cập nhật thông tin nhân viên.
    /// </summary>
    public class UpdateEmployeeRequestDto
    {
        /// <summary>
        /// Tên nhân viên.
        /// </summary>
        [MaxLength(100)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Tên đệm nhân viên.
        /// </summary>
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Họ nhân viên.
        /// </summary>
        [MaxLength(100)]
        public string? LastName { get; set; }

        /// <summary>
        /// Tuổi nhân viên.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Ngày sinh nhân viên.
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Địa chỉ liên lạc của nhân viên.
        /// </summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// Địa chỉ email của nhân viên.
        /// </summary>
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Id phòng ban mà nhân viên thuộc về.
        /// </summary>
        [MaxLength(450)]
        public string? Department { get; set; }

        /// <summary>
        /// Ngày bắt đầu làm việc.
        /// </summary>
        public DateTime? WorkStartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc làm việc.
        /// </summary>
        public DateTime? WorkEndDate { get; set; }
    }
}
