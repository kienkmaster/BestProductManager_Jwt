using System.ComponentModel.DataAnnotations;

namespace BestProductManager.Api.Dtos.Products
{
    /// <summary>
    /// DTO dùng để trao đổi dữ liệu sản phẩm giữa API và client.
    /// </summary>
    public class UpdProductDto
    {
        /// <summary>
        /// Tên sản phẩm.
        /// </summary>
        [Required]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Giá bán.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Số lượng tồn kho.
        /// </summary>
        public int Stock { get; set; }
    }
}
