import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình Trang chủ (Home).
 * - Khi chưa đăng nhập: hiển thị form Đăng nhập với các trường username/password.
 * - Khi đã đăng nhập: hiển thị lời chào.
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  /** Tên đăng nhập người dùng nhập trên form. */
  userName = '';

  /** Mật khẩu người dùng nhập trên form. */
  password = '';

  /** Thông báo lỗi khi đăng nhập không thành công. */
  loginError: string | null = null;

  /**
   * Khởi tạo HomeComponent với AuthService và Router.
   * @param authService Dịch vụ xác thực dùng để gửi yêu cầu đăng nhập.
   * @param router Dịch vụ định tuyến dùng để điều hướng sau khi login.
   */
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  /**
   * Thực hiện đăng nhập khi người dùng nhấn nút "Đăng nhập".
   * Gọi AuthService.login, nếu thành công thì chuyển đến màn danh sách sản phẩm.
   */
  async onLogin(): Promise<void> {
    this.loginError = null;

    try {
      await this.authService.login(this.userName, this.password);

      this.userName = '';
      this.password = '';

      await this.router.navigate(['/products']);
    }
    catch (error: any) {
      this.loginError = error?.message ?? 'Tên đăng nhập hoặc mật khẩu không hợp lệ.';
    }
  }

  /**
   * Điều hướng sang màn hình Đăng ký khi người dùng nhấn nút "Đăng ký".
   */
  async onGoToRegister(): Promise<void> {
    await this.router.navigate(['/register']);
  }

  /**
   * Cho biết người dùng hiện đã đăng nhập hay chưa.
   */
  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  /**
   * Lấy tên người dùng hiện tại (nếu đã đăng nhập).
   */
  get currentUser(): string | null {
    return this.authService.userName;
  }
}
