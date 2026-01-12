/**
 * Kiểu dữ liệu mô tả thông tin phân loại role hiện tại của một user.
 * Dùng cho màn hình Thay đổi phân loại thành viên.
 */
export interface UserRoleInfo {
    /** Khóa chính của user. */
    userId: string;
  
    /** Tên đăng nhập của user. */
    userName: string;
  
    /** Khóa chính của role hiện tại. */
    roleId: string | null;
  
    /** Tên role hiện tại. */
    roleName: string | null;
  }
  