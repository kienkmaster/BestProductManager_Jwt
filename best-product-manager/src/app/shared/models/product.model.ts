/**
 * Kiểu dữ liệu Product dùng trên frontend để hiển thị và trao đổi với API.
 * Khớp với ProductDto trên backend.
 */
export interface Product {
  /** Khóa chính của sản phẩm. */
  id: number;

  /** Tên sản phẩm. */
  productName: string;

  /** Giá bán của sản phẩm. */
  price: number;

  /** Số lượng tồn kho hiện có. */
  stock: number;
}
