/**
 * Model phản hồi dạng message (ApiMessageResponseModel).
 * Mục tiêu: ánh xạ payload trả về từ API khi thao tác thành công/thất bại.
 */
export class ApiMessageResponseModel {
  /**
   * Khởi tạo model phản hồi.
   * @param message Nội dung message từ backend (backend message).
   */
  constructor(
    public readonly message: string
  ) {
    /**
     * Model thuần dữ liệu: không xử lý nghiệp vụ trong constructor.
     */
  }
}
