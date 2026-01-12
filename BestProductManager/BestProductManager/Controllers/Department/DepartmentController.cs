using BestProductManager.Dtos.Department;
using BestProductManager.Services.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestProductManager.Api.Controllers.Department
{
    /// <summary>
    /// API quản lý phòng ban (M_Department).
    /// Chỉ cho phép user thuộc role admin truy cập.
    /// </summary>
    [ApiController]
    [Route("api/department")]
    [Authorize(Roles = "admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        /// <summary>
        /// Khởi tạo DepartmentController với DepartmentService và Logger.
        /// </summary>
        /// <param name="departmentService">Dịch vụ nghiệp vụ phòng ban.</param>
        /// <param name="logger">Logger dùng để ghi log cho các thao tác phòng ban.</param>
        public DepartmentController(
            IDepartmentService departmentService,
            ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        /// <summary>
        /// Đăng ký phòng ban mới.
        /// Chỉ role admin được phép gọi.
        /// </summary>
        /// <param name="request">Thông tin phòng ban cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id phòng ban mới được tạo.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterDepartment(
            [FromBody] CreateDepartmentRequestDto request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu đăng ký phòng ban không hợp lệ.",
                    errors = ModelState
                });
            }

            var newDepartmentId = await _departmentService.CreateDepartmentAsync(request, cancellationToken);

            _logger.LogInformation(
                "Admin {AdminName} đã tạo mới phòng ban {DepartmentName} với Id {DepartmentId}.",
                User?.Identity?.Name,
                request.Name,
                newDepartmentId);

            return Ok(new
            {
                message = "Đăng ký phòng ban thành công.",
                id = newDepartmentId
            });
        }

        /// <summary>
        /// Cập nhật thông tin phòng ban.
        /// Chỉ role admin được phép gọi.
        /// </summary>
        /// <param name="id">Id phòng ban cần cập nhật.</param>
        /// <param name="request">Thông tin tên phòng ban mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDepartment(
            [FromRoute] string id,
            [FromBody] UpdateDepartmentRequestDto request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu cập nhật phòng ban không hợp lệ.",
                    errors = ModelState
                });
            }

            var updated = await _departmentService.UpdateDepartmentAsync(id, request, cancellationToken);
            if (!updated)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng ban cần cập nhật."
                });
            }

            _logger.LogInformation(
                "Admin {AdminName} đã cập nhật phòng ban Id {DepartmentId} với tên mới {DepartmentName}.",
                User?.Identity?.Name,
                id,
                request.Name);

            return Ok(new
            {
                message = "Cập nhật phòng ban thành công."
            });
        }

        /// <summary>
        /// Xóa phòng ban theo Id.
        /// Chỉ role admin được phép gọi.
        /// </summary>
        /// <param name="id">Id phòng ban cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDepartment(
            [FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var deleted = await _departmentService.DeleteDepartmentAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng ban cần xóa."
                });
            }

            _logger.LogInformation(
                "Admin {AdminName} đã xóa phòng ban Id {DepartmentId}.",
                User?.Identity?.Name,
                id);

            return Ok(new
            {
                message = "Xóa phòng ban thành công."
            });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phòng ban.
        /// Chỉ role admin được phép gọi.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách DepartmentDto.</returns>
        [HttpGet("getalldepartments")]
        public async Task<ActionResult<IReadOnlyCollection<DepartmentDto>>> GetAllDepartments(
            CancellationToken cancellationToken)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
            return Ok(departments);
        }

        /// <summary>
        /// Tìm kiếm phòng ban theo Id và/hoặc tên phòng ban.
        /// Nếu nhập cả hai trường, kết quả phải đồng thời thỏa:
        /// - Id khớp với "Id phòng ban".
        /// - Tên phòng ban chứa chuỗi tìm kiếm từ "Tên phòng ban" (so sánh không phân biệt hoa thường).
        /// </summary>
        /// <param name="departmentId">Giá trị nhập từ textbox "Id phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="name">Giá trị nhập từ textbox "Tên phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách phòng ban thỏa điều kiện tìm kiếm.</returns>
        [HttpGet("searchdepartments")]
        public async Task<ActionResult<IReadOnlyCollection<DepartmentDto>>> SearchDepartments(
            [FromQuery] string? departmentId,
            [FromQuery] string? name,
            CancellationToken cancellationToken)
        {
            var departments = await _departmentService.SearchDepartmentsAsync(
                departmentId,
                name,
                cancellationToken);

            return Ok(departments);
        }
    }
}
