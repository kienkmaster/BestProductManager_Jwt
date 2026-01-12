using Microsoft.AspNetCore.Identity;

namespace BestProductManager.Api.Entities
{
    /// <summary>
    /// Kiểu người dùng ứng dụng kế thừa từ IdentityUser để sử dụng ASP.NET Core Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
    }
}
