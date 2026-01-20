/**
 * Model đăng ký user (RegisterUserModel).
 * Mục tiêu: biểu diễn dữ liệu request (DTO-like) gửi lên API.
 */
export class RegisterUserModel {
  /**
   * Khởi tạo model đăng ký.
   * @param userName Tên người dùng (userName) gửi API.
   * @param password Mật khẩu (password) gửi API.
   * @param confirmPassword Xác nhận mật khẩu (confirmPassword) gửi API.
   */
  constructor(
    public readonly userName: string,
    public readonly password: string,
    public readonly confirmPassword: string
  ) {
    /**
     * Model thuần dữ liệu: không xử lý nghiệp vụ trong constructor.
     */
  }
}
