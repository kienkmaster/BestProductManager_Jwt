import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình Liên hệ (Contact).
 * Hiển thị thông tin liên hệ dựa trên user đang đăng nhập:
 * - Tên userName hiện tại.
 * Nếu chưa đăng nhập, hiển thị thông báo yêu cầu đăng nhập.
 */
@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent {
  /**
   * Khởi tạo ContactComponent với AuthService.
   * @param authService Dịch vụ xác thực dùng để lấy thông tin user hiện tại.
   */
  constructor(public authService: AuthService) { }

  /**
   * Cho biết người dùng hiện đã đăng nhập hay chưa.
   * Dùng để điều khiển hiển thị nội dung liên hệ.
   */
  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  /**
   * Lấy tên userName hiện tại của người dùng đăng nhập.
   */
  get currentUserName(): string | null {
    return this.authService.userName;
  }
}
