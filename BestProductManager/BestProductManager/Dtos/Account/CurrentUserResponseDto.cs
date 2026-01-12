using System;
using System.Collections.Generic;

namespace BestProductManager.Dtos.Account
{
    /// <summary>
    /// DTO trả về thông tin người dùng hiện tại và danh sách role tương ứng.
    /// </summary>
    public sealed class CurrentUserResponseDto
    {
        /// <summary>
        /// Tên đăng nhập của người dùng hiện tại.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Danh sách role mà người dùng hiện tại đang sở hữu.
        /// </summary>
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    }
}
