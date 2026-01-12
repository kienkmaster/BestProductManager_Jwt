/**
 * Kiểu dữ liệu mô tả một role trong hệ thống.
 * Dùng để bind vào combobox phân loại thành viên.
 */
export interface RoleOption {
    /** Khóa chính của role (Id trong bảng SecRole). */
    id: string;
  
    /** Tên role hiển thị (Name trong bảng SecRole). */
    name: string;
  }
  