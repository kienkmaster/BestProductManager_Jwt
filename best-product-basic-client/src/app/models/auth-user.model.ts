/**
 * Model (DTO-like) đại diện cho thông tin user sau khi đăng nhập.
 */
export class AuthUserModel {
  /**
   * Khởi tạo AuthUserModel.
   * @param userName Tên user đã đăng nhập.
   * @param roles Danh sách role của user.
   */
  constructor(
    public readonly userName: string,
    public readonly roles: string[]
  ) {}
}
