using System.Data;
using System.Data.Common;
using BestProductManager.Api.Data;
using BestProductManager.Dtos.Employee;
using BestProductManager.Entities.Employee;
using Dapper;

namespace BestProductManager.Repository.Employee
{
    /// <summary>
    /// Triển khai repository thao tác dữ liệu nhân viên sử dụng Dapper.
    /// Thực hiện truy vấn thông qua stored procedure thuộc schema [Employee].
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Factory tạo kết nối SQL Server dùng cho Dapper.
        /// </summary>
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        /// <summary>
        /// Khởi tạo EmployeeRepository với SqlConnectionFactory.
        /// </summary>
        /// <param name="sqlConnectionFactory">Factory tạo kết nối cơ sở dữ liệu cho Dapper.</param>
        public EmployeeRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <inheritdoc />
        public async Task<int> GetMaxEmployeeIdAsync(CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                "[Employee].[GetMaxEmployeeId]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var maxId = await connection.ExecuteScalarAsync<int>(command);
            return maxId;
        }

        /// <inheritdoc />
        public async Task<int> CreateEmployeeAsync(EmployeeEntity employee, CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Id", employee.Id, DbType.String);
            parameters.Add("FirstName", employee.FirstName, DbType.String);
            parameters.Add("MiddleName", employee.MiddleName, DbType.String);
            parameters.Add("LastName", employee.LastName, DbType.String);
            parameters.Add("Age", employee.Age, DbType.Int32);
            parameters.Add("Birthday", employee.Birthday, DbType.Date);
            parameters.Add("Address", employee.Address, DbType.String);
            parameters.Add("Email", employee.Email, DbType.String);
            parameters.Add("Department", employee.Department, DbType.String);
            parameters.Add("WorkStartDate", employee.WorkStartDate, DbType.DateTime2);
            parameters.Add("WorkEndDate", employee.WorkEndDate, DbType.DateTime2);

            var affected = await connection.ExecuteAsync(
                "[Employee].[CreateEmployee]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return affected;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateEmployeeAsync(EmployeeEntity employee, CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Id", employee.Id, DbType.String);
            parameters.Add("FirstName", employee.FirstName, DbType.String);
            parameters.Add("MiddleName", employee.MiddleName, DbType.String);
            parameters.Add("LastName", employee.LastName, DbType.String);
            parameters.Add("Age", employee.Age, DbType.Int32);
            parameters.Add("Birthday", employee.Birthday, DbType.Date);
            parameters.Add("Address", employee.Address, DbType.String);
            parameters.Add("Email", employee.Email, DbType.String);
            parameters.Add("Department", employee.Department, DbType.String);
            parameters.Add("WorkStartDate", employee.WorkStartDate, DbType.DateTime2);
            parameters.Add("WorkEndDate", employee.WorkEndDate, DbType.DateTime2);

            var affected = await connection.ExecuteAsync(
                "[Employee].[UpdateEmployee]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEmployeeAsync(string id, CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.String);

            var affected = await connection.ExecuteAsync(
                "[Employee].[DeleteEmployee]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
            await ((DbConnection)connection).OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                "[Employee].[GetAllEmployees]",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var employees = await connection.QueryAsync<EmployeeDto>(command);
            return employees.ToList();
        }
    }
}
