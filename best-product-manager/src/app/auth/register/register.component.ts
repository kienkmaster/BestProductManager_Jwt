import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình Đăng ký tài khoản.
 * Gọi API BestProductManager để tạo người dùng mới trong bảng Users.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  /** Tên đăng nhập trên form đăng ký. */
  userName = '';

  /** Mật khẩu trên form đăng ký. */
  password = '';

  /** Mật khẩu xác nhận. */
  confirmPassword = '';

  /** Thông báo lỗi (nếu có). */
  errorMessage: string | null = null;

  /** Thông báo thành công (nếu đăng ký thành công). */
  successMessage: string | null = null;

  /**
   * Khởi tạo RegisterComponent với AuthService.
   * @param authService Dịch vụ xác thực dùng để gửi yêu cầu đăng ký user mới.
   */
  constructor(private authService: AuthService) { }

  /**
   * Xử lý khi người dùng nhấn nút "Đăng ký".
   * Gọi AuthService.register, hiển thị thông báo tương ứng.
   */
  async onRegister(): Promise<void> {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.userName.trim() || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Vui lòng nhập đầy đủ Tên đăng nhập, Mật khẩu và Xác nhận mật khẩu.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Mật khẩu và Xác nhận mật khẩu không khớp.';
      return;
    }

    try {
      await this.authService.register(this.userName, this.password, this.confirmPassword);
      this.successMessage = 'Đăng ký tài khoản thành công.';

      this.userName = '';
      this.password = '';
      this.confirmPassword = '';
    }
    catch (error: any) {
      this.errorMessage = error?.message ?? 'Đăng ký tài khoản không thành công.';
    }
  }
}
