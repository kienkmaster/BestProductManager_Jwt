using BestProductManager.Dtos.Employee;
using BestProductManager.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestProductManager.Api.Controllers.Employee
{
    /// <summary>
    /// API quản lý nhân viên (M_Employee).
    /// Chỉ cho phép user có role admin truy cập.
    /// </summary>
    [ApiController]
    [Route("api/employee")]
    [Authorize(Roles = "admin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        /// <summary>
        /// Khởi tạo EmployeeController với EmployeeService và Logger.
        /// </summary>
        /// <param name="employeeService">Dịch vụ nghiệp vụ quản lý nhân viên.</param>
        /// <param name="logger">Logger dùng để ghi log cho các hoạt động quản lý nhân viên.</param>
        public EmployeeController(
            IEmployeeService employeeService,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Đăng ký nhân viên mới.
        /// RBAC: chỉ cho phép role admin.
        /// </summary>
        /// <param name="dto">Thông tin nhân viên cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Thông tin Id nhân viên mới tạo.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] CreateEmployeeRequestDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu đăng ký nhân viên không hợp lệ.",
                    errors = ModelState
                });
            }

            var newId = await _employeeService.RegisterAsync(dto, cancellationToken);

            _logger.LogInformation(
                "Admin {AdminName} đã đăng ký nhân viên mới Id = {EmployeeId}.",
                User?.Identity?.Name,
                newId);

            var url = Url.Action(nameof(GetAllEmployees), "Employee", null, Request.Scheme) ?? string.Empty;

            return Created(url, new
            {
                success = true,
                newId
            });
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên.
        /// RBAC: chỉ cho phép role admin.
        /// </summary>
        /// <param name="id">Id nhân viên cần cập nhật.</param>
        /// <param name="dto">Thông tin cập nhật.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(
            [FromRoute] string id,
            [FromBody] UpdateEmployeeRequestDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu cập nhật nhân viên không hợp lệ.",
                    errors = ModelState
                });
            }

            var updated = await _employeeService.UpdateAsync(id, dto, cancellationToken);
            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy nhân viên cần cập nhật."
                });
            }

            _logger.LogInformation(
                "Admin {AdminName} đã cập nhật thông tin nhân viên Id = {EmployeeId}.",
                User?.Identity?.Name,
                id);

            return Ok(new
            {
                success = true
            });
        }

        /// <summary>
        /// Xóa nhân viên theo Id.
        /// RBAC: chỉ cho phép role admin.
        /// </summary>
        /// <param name="id">Id nhân viên cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(
            [FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var deleted = await _employeeService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy nhân viên cần xóa."
                });
            }

            _logger.LogInformation(
                "Admin {AdminName} đã xóa nhân viên Id = {EmployeeId}.",
                User?.Identity?.Name,
                id);

            return Ok(new
            {
                success = true
            });
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên.
        /// RBAC: chỉ cho phép role admin.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách nhân viên.</returns>
        [HttpGet("getallemployees")]
        public async Task<IActionResult> GetAllEmployees(
            CancellationToken cancellationToken)
        {
            var employees = await _employeeService.GetAllAsync(cancellationToken);

            return Ok(new
            {
                success = true,
                data = employees
            });
        }

        /// <summary>
        /// Tìm kiếm nhân viên theo nhiều điều kiện.
        /// RBAC: chỉ cho phép role admin.
        /// </summary>
        /// <param name="request">Điều kiện tìm kiếm (Id, FirstName, MiddleName, LastName, Age, Birthday, Address, Email, Department).</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách nhân viên thỏa điều kiện.</returns>
        [HttpGet("searchemployees")]
        public async Task<IActionResult> SearchEmployees(
            [FromQuery] SearchEmployeesRequestDto request,
            CancellationToken cancellationToken)
        {
            var employees = await _employeeService.SearchAsync(request, cancellationToken);

            return Ok(new
            {
                success = true,
                data = employees
            });
        }
    }
}
