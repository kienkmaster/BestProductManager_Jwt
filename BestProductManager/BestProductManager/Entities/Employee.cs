namespace BestProductManager.Entities.Employee
{
    /// <summary>
    /// Thực thể nhân viên, ánh xạ với bảng M_Employee.
    /// </summary>
    public class EmployeeEntity
    {
        /// <summary>
        /// Khóa chính nhân viên (Id trong M_Employee).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tên nhân viên (FirstName).
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Tên đệm nhân viên (MiddleName).
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Họ nhân viên (LastName).
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Tuổi nhân viên (Age).
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Ngày sinh nhân viên (Birthday).
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Địa chỉ liên lạc của nhân viên (Address).
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Địa chỉ email của nhân viên (Email).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Id phòng ban mà nhân viên thuộc về (Department, FK tới M_Department.Id).
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Ngày bắt đầu làm việc (WorkStartDate).
        /// </summary>
        public DateTime? WorkStartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc làm việc (WorkEndDate, null nếu còn làm).
        /// </summary>
        public DateTime? WorkEndDate { get; set; }

        /// <summary>
        /// Thời điểm tạo record nhân viên (CreatedDate).
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất record nhân viên (UpdatedDate).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
