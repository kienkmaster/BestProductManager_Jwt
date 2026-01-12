import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from './core/services/auth.service';

/**
 * Root component của ứng dụng.
 * Hiển thị:
 * - Tiêu đề "Best Product Manager".
 * - Thanh điều hướng với các menu chức năng.
 * - router-outlet để chứa các màn hình con.
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NgIf
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  /** Tiêu đề chính hiển thị ở phần header. */
  title = 'Best Product Manager';

  /**
   * Khởi tạo AppComponent với AuthService và Router.
   * @param authService Dịch vụ xác thực dùng để đọc trạng thái đăng nhập và role hiện tại.
   * @param router Dịch vụ định tuyến dùng để điều hướng giữa các màn hình.
   */
  constructor(
    public authService: AuthService,
    private router: Router
  ) { }

  /**
   * Khi ứng dụng khởi động, kiểm tra lại trạng thái đăng nhập và role dựa trên cookie hiện có.
   * Mục đích: nếu user refresh trang, giao diện vẫn giữ đúng trạng thái login/logout và phân quyền.
   */
  async ngOnInit(): Promise<void> {
    await this.authService.refreshLoginStatusFromApi();
  }

  /**
   * Xử lý khi người dùng nhấn nút Đăng xuất trên thanh điều hướng.
   * Gọi AuthService.logout để xóa cookie xác thực và điều hướng về Trang chủ.
   */
  async onLogout(): Promise<void> {
    await this.authService.logout();
    await this.router.navigate(['/']);
  }
}
