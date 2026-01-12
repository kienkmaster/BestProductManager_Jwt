using BestProductManager.Dtos.Department;
using BestProductManager.Repositories;

namespace BestProductManager.Services.Department
{
    /// <summary>
    /// Triển khai dịch vụ nghiệp vụ phòng ban.
    /// Kết hợp repository để thao tác database và áp dụng rule nghiệp vụ tìm kiếm.
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        /// <summary>
        /// Khởi tạo DepartmentService với DepartmentRepository.
        /// </summary>
        /// <param name="departmentRepository">Repository thao tác dữ liệu phòng ban.</param>
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Đăng ký phòng ban mới.
        /// </summary>
        /// <param name="request">Thông tin phòng ban cần đăng ký.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id phòng ban mới được tạo.</returns>
        public async Task<string> CreateDepartmentAsync(
            CreateDepartmentRequestDto request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var trimmedName = request.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Tên phòng ban không được rỗng.", nameof(request.Name));
            }

            var newId = await _departmentRepository.CreateDepartmentAsync(trimmedName, cancellationToken);
            return newId;
        }

        /// <summary>
        /// Cập nhật thông tin phòng ban.
        /// </summary>
        /// <param name="id">Id phòng ban cần cập nhật.</param>
        /// <param name="request">Thông tin tên phòng ban mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại false.</returns>
        public async Task<bool> UpdateDepartmentAsync(
            string id,
            UpdateDepartmentRequestDto request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var trimmedId = id?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedId))
            {
                throw new ArgumentException("Id phòng ban không được rỗng.", nameof(id));
            }

            var trimmedName = request.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new ArgumentException("Tên phòng ban không được rỗng.", nameof(request.Name));
            }

            var updated = await _departmentRepository.UpdateDepartmentAsync(
                trimmedId,
                trimmedName,
                cancellationToken);

            return updated;
        }

        /// <summary>
        /// Xóa phòng ban theo Id.
        /// </summary>
        /// <param name="id">Id phòng ban cần xóa.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu xóa thành công, ngược lại false.</returns>
        public async Task<bool> DeleteDepartmentAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var trimmedId = id?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedId))
            {
                throw new ArgumentException("Id phòng ban không được rỗng.", nameof(id));
            }

            var deleted = await _departmentRepository.DeleteDepartmentAsync(trimmedId, cancellationToken);
            return deleted;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phòng ban.
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách DepartmentDto.</returns>
        public Task<IReadOnlyCollection<DepartmentDto>> GetAllDepartmentsAsync(
            CancellationToken cancellationToken = default)
        {
            return _departmentRepository.GetAllDepartmentsAsync(cancellationToken);
        }

        /// <summary>
        /// Tìm kiếm phòng ban theo Id và/hoặc tên phòng ban.
        /// Nếu nhập cả hai trường, kết quả phải đồng thời thỏa:
        /// - Id khớp chính xác với "Id phòng ban".
        /// - Tên phòng ban chứa chuỗi tìm kiếm từ "Tên phòng ban" (không phân biệt hoa thường).
        /// </summary>
        /// <param name="departmentId">Giá trị nhập từ "Id phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="name">Giá trị nhập từ "Tên phòng ban". Có thể null hoặc rỗng.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách phòng ban thỏa điều kiện tìm kiếm.</returns>
        public async Task<IReadOnlyCollection<DepartmentDto>> SearchDepartmentsAsync(
            string? departmentId,
            string? name,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hasIdFilter = !string.IsNullOrWhiteSpace(departmentId);
            var hasNameFilter = !string.IsNullOrWhiteSpace(name);

            if (!hasIdFilter && !hasNameFilter)
            {
                return Array.Empty<DepartmentDto>();
            }

            var allDepartments = await _departmentRepository.GetAllDepartmentsAsync(cancellationToken);

            IEnumerable<DepartmentDto> query = allDepartments;

            if (hasIdFilter)
            {
                var trimmedId = departmentId!.Trim();
                query = query.Where(d =>
                    string.Equals(d.Id, trimmedId, StringComparison.OrdinalIgnoreCase));
            }

            if (hasNameFilter)
            {
                var trimmedName = name!.Trim();

                query = query.Where(d =>
                    !string.IsNullOrWhiteSpace(d.Name) &&
                    d.Name.Contains(trimmedName, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }
    }
}
