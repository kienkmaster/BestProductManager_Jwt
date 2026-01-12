using System;
using System.Collections.Generic;

namespace BestProductManager.Api.Dtos.Account
{
    /// <summary>
    /// DTO trả về kết quả đăng nhập bao gồm thông tin người dùng và JWT access token.
    /// </summary>
    public sealed class LoginResponseDto
    {
        /// <summary>
        /// Tên đăng nhập của người dùng.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách role hiện tại của người dùng.
        /// </summary>
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Chuỗi JWT access token mà client sử dụng ở header Authorization.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }
}
