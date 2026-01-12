using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestProductManager.Api.Entities
{
    /// <summary>
    /// Thực thể Product biểu diễn sản phẩm trong hệ thống BestProductManager.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Khóa chính của sản phẩm.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tên sản phẩm.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Đơn giá của sản phẩm.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999)]
        public decimal Price { get; set; }

        /// <summary>
        /// Số lượng tồn kho hiện tại.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
