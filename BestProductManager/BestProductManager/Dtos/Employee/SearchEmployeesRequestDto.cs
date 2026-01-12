namespace BestProductManager.Dtos.Employee
{
    /// <summary>
    /// DTO yêu cầu tìm kiếm nhân viên theo nhiều điều kiện.
    /// Tất cả các field đều tùy chọn, nếu null thì không đưa vào điều kiện lọc.
    /// </summary>
    public class SearchEmployeesRequestDto
    {
        /// <summary>
        /// Id nhân viên cần tìm kiếm (so khớp chính xác).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Tên nhân viên (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Tên đệm nhân viên (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Họ nhân viên (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Tuổi nhân viên (so khớp chính xác).
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Ngày sinh nhân viên (so khớp theo ngày).
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Địa chỉ (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Địa chỉ email (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Id phòng ban (lọc theo chứa chuỗi, không phân biệt hoa thường).
        /// </summary>
        public string? Department { get; set; }
    }
}
