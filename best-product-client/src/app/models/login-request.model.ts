/**
 * Model (DTO-like) cho request đăng nhập.
 */
export class LoginRequestModel {
  /**
   * Khởi tạo LoginRequestModel.
   * @param userName Tên đăng nhập gửi API.
   * @param password Mật khẩu gửi API.
   */
  constructor(
    public readonly userName: string,
    public readonly password: string
  ) {}
}
