using System.Globalization;
using BestProductManager.Dtos.Employee;
using BestProductManager.Entities.Employee;
using BestProductManager.Repository.Employee;

namespace BestProductManager.Services.Employee
{
    /// <summary>
    /// Triển khai dịch vụ nghiệp vụ quản lý nhân viên.
    /// Thực hiện mapping giữa DTO và entity, gọi repository Dapper.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Khởi tạo EmployeeService với repository nhân viên.
        /// </summary>
        /// <param name="employeeRepository">Repository thao tác dữ liệu nhân viên.</param>
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <inheritdoc />
        public async Task<string> RegisterAsync(
            CreateEmployeeRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var maxId = await _employeeRepository.GetMaxEmployeeIdAsync(cancellationToken);
            var newNumericId = maxId + 1;
            var newId = newNumericId.ToString(CultureInfo.InvariantCulture);

            var employee = new EmployeeEntity
            {
                Id = newId,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Age = dto.Age,
                Birthday = dto.Birthday,
                Address = dto.Address,
                Email = dto.Email,
                Department = dto.Department,
                WorkStartDate = dto.WorkStartDate,
                WorkEndDate = dto.WorkEndDate
            };

            await _employeeRepository.CreateEmployeeAsync(employee, cancellationToken);

            return newId;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(
            string id,
            UpdateEmployeeRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var employee = new EmployeeEntity
            {
                Id = id,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Age = dto.Age,
                Birthday = dto.Birthday,
                Address = dto.Address,
                Email = dto.Email,
                Department = dto.Department,
                WorkStartDate = dto.WorkStartDate,
                WorkEndDate = dto.WorkEndDate
            };

            var updated = await _employeeRepository.UpdateEmployeeAsync(employee, cancellationToken);
            return updated;
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _employeeRepository.DeleteEmployeeAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _employeeRepository.GetAllEmployeesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<EmployeeDto>> SearchAsync(
            SearchEmployeesRequestDto request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var all = await _employeeRepository.GetAllEmployeesAsync(cancellationToken);
            var query = all.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                query = query.Where(e =>
                    string.Equals(e.Id, request.Id, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.FirstName) &&
                    e.FirstName.Contains(request.FirstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.MiddleName))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.MiddleName) &&
                    e.MiddleName.Contains(request.MiddleName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.LastName) &&
                    e.LastName.Contains(request.LastName, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Age.HasValue)
            {
                query = query.Where(e => e.Age == request.Age);
            }

            if (request.Birthday.HasValue)
            {
                var targetDate = request.Birthday.Value.Date;
                query = query.Where(e =>
                    e.Birthday.HasValue &&
                    e.Birthday.Value.Date == targetDate);
            }

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.Address) &&
                    e.Address.Contains(request.Address, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.Email) &&
                    e.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Department))
            {
                query = query.Where(e =>
                    !string.IsNullOrWhiteSpace(e.Department) &&
                    e.Department.Contains(request.Department, StringComparison.OrdinalIgnoreCase));
            }

            var result = query.ToList();
            return result;
        }
    }
}
