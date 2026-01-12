using System.Data;
using System.Data.Common;
using BestProductManager.Api.Data;
using BestProductManager.Dtos.Department;
using Dapper;

namespace BestProductManager.Repositories
{
    /// <summary>
    /// Triển khai repository truy xuất dữ liệu cho thực thể phòng ban (M_Department) sử dụng Dapper.
    /// Thực hiện truy vấn thông qua stored procedure thuộc schema Department.
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        /// <summary>
        /// Factory tạo kết nối SQL Server dùng cho Dapper.
        /// </summary>
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        /// <summary>
        /// Khởi tạo DepartmentRepository với SqlConnectionFactory.
        /// </summary>
        /// <param name="sqlConnectionFactory">Factory tạo kết nối cơ sở dữ liệu cho Dapper.</param>
        public DepartmentRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Tạo mới phòng ban.
        /// Rule:
        /// - Gọi stored [Department].[GetMaxDepartmentId] để lấy Id lớn nhất hiện tại.
        /// - Cộng thêm 1 để tạo Id mới.
        /// - Ghi dữ liệu bằng stored [Department].[CreateDepartment].
        /// </summary>
        /// <param name="name">Tên phòng ban.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Id phòng ban mới dưới dạng chuỗi.</returns>
        public async Task<string> CreateDepartmentAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var getMaxCommand = new CommandDefinition(
                "[Department].[GetMaxDepartmentId]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var currentMaxId = await connection.ExecuteScalarAsync<int?>(
                getMaxCommand);

            var newIdInt = (currentMaxId ?? 0) + 1;
            var newId = newIdInt.ToString();

            var insertParameters = new DynamicParameters();
            insertParameters.Add("Id", newId);
            insertParameters.Add("Name", name);

            var insertCommand = new CommandDefinition(
                "[Department].[CreateDepartment]",
                insertParameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(insertCommand);

            return newId;
        }

        /// <summary>
        /// Cập nhật tên phòng ban theo Id.
        /// </summary>
        /// <param name="id">Id phòng ban cần cập nhật.</param>
        /// <param name="name">Tên phòng ban mới.</param>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại false.</returns>
        public async Task<bool> UpdateDepartmentAsync(
            string id,
            string name,
            CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", name);

            var command = new CommandDefinition(
                "[Department].[UpdateDepartment]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var affected = await connection.ExecuteAsync(command);
            return affected > 0;
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
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var command = new CommandDefinition(
                "[Department].[DeleteDepartment]",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var affected = await connection.ExecuteAsync(command);
            return affected > 0;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phòng ban từ stored [Department].[GetAllDepartments].
        /// </summary>
        /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
        /// <returns>Danh sách DepartmentDto.</returns>
        public async Task<IReadOnlyCollection<DepartmentDto>> GetAllDepartmentsAsync(
            CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                "[Department].[GetAllDepartments]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var departments = await connection.QueryAsync<DepartmentDto>(command);
            return departments.ToList();
        }
    }
}
