namespace BestProductManager.Dtos.Employee
{
    /// <summary>
    /// DTO mô tả thông tin nhân viên dùng cho màn hình tra cứu, danh sách.
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Khóa chính nhân viên.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên nhân viên.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Tên đệm nhân viên.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Họ nhân viên.
        /// </summary>
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
        /// Địa chỉ liên lạc.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Địa chỉ email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Id phòng ban mà nhân viên thuộc về.
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Ngày bắt đầu làm việc.
        /// </summary>
        public DateTime? WorkStartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc làm việc.
        /// </summary>
        public DateTime? WorkEndDate { get; set; }

        /// <summary>
        /// Thời điểm tạo record.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất record.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
