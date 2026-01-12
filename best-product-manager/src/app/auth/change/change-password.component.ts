import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình Đổi mật khẩu cho chính người dùng đang đăng nhập.
 * Hiển thị:
 * - Mật khẩu cũ
 * - Mật khẩu mới
 * - Xác nhận mật khẩu mới
 * Và sử dụng rule đặt password giống với màn đăng ký (do backend kiểm tra).
 */
@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  /** Mật khẩu cũ người dùng đang sử dụng. */
  currentPassword = '';

  /** Mật khẩu mới người dùng muốn thiết lập. */
  newPassword = '';

  /** Mật khẩu mới dùng để xác nhận (nhập lại). */
  confirmNewPassword = '';

  /** Thông báo lỗi khi đổi mật khẩu không thành công. */
  errorMessage: string | null = null;

  /** Thông báo thành công khi đổi mật khẩu thành công. */
  successMessage: string | null = null;

  /**
   * Khởi tạo ChangePasswordComponent với AuthService.
   * @param authService Dịch vụ xác thực dùng để gửi yêu cầu đổi mật khẩu.
   */
  constructor(private authService: AuthService) { }

  /**
   * Xử lý khi người dùng nhấn nút "Xác nhận".
   * Gọi AuthService.changePassword và hiển thị kết quả.
   */
  async onSubmit(): Promise<void> {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.currentPassword || !this.newPassword || !this.confirmNewPassword) {
      this.errorMessage = 'Vui lòng nhập đầy đủ Mật khẩu cũ, Mật khẩu mới và Xác nhận mật khẩu mới.';
      return;
    }

    if (this.newPassword !== this.confirmNewPassword) {
      this.errorMessage = 'Mật khẩu mới và Xác nhận mật khẩu mới không khớp.';
      return;
    }

    try {
      await this.authService.changePassword(
        this.currentPassword,
        this.newPassword,
        this.confirmNewPassword
      );

      this.successMessage = 'Đổi mật khẩu thành công.';
      this.currentPassword = '';
      this.newPassword = '';
      this.confirmNewPassword = '';
    }
    catch (error: any) {
      this.errorMessage = error?.message ?? 'Đổi mật khẩu không thành công.';
    }
  }
}
