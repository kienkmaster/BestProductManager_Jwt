using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestProductManager.Api.Entities
{
    /// <summary>
    /// ApiResponse
    /// </summary>
    public class ApiResponse<T>
    {
        // API call thanh cong hay that bai
        public bool Success { get; set; }
        //Data tra cho Client
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        // Loi chi tiet (neu co)
        public string Error { get; set; } = "";
        // Thoi diem tao respone
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Response thanh cong
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Success",
                Data = data,
                Error = ""
            };
        }

        /// <summary>
        /// Response that bai
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> Fail(string error, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message ?? "Failed",
                Data = default(T),
                Error = error                   // Loi tra ve Client
            };
        }
    }
}
