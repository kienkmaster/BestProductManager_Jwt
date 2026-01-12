/**
 * Kiểu dữ liệu dùng để hiển thị danh sách thành viên trong màn Quản lý thành viên.
 * Dữ liệu này được lấy từ API quản trị (admin).
 */
export interface MemberSummary {
  /** Id nội bộ của user (khóa chính, dùng cho thao tác admin). */
  id: string;

  /** Tên đăng nhập của user. */
  userName: string;

  /** Cờ cho biết user có thuộc role Admin hay không. */
  isAdmin: boolean;

  /** Khóa chính của role chính hiện tại (Id trong bảng SecRole). */
  roleId?: string | null;

  /** Tên role chính hiện tại (Name trong bảng SecRole). */
  roleName?: string | null;
}
