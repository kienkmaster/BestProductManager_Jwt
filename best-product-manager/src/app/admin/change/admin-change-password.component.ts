import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserAdminService } from '../../core/services/user-admin.service';

/**
 * Màn hình Đổi mật khẩu do Admin sử dụng để đổi mật khẩu cho user khác.
 * Layout:
 * - Mật khẩu mới
 * - Nút "Áp dụng"
 */
@Component({
  selector: 'app-admin-change-password',
  standalone: true,
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './admin-change-password.component.html',
  styleUrl: './admin-change-password.component.css'
})
export class AdminChangePasswordComponent implements OnInit {
  /** Id của user mục tiêu được lấy từ route parameter. */
  targetUserId: string | null = null;

  /** Tên đăng nhập của user mục tiêu (lấy từ query string để hiển thị cho dễ hiểu). */
  targetUserName: string | null = null;

  /** Mật khẩu mới sẽ được admin áp dụng cho user mục tiêu. */
  newPassword = '';

  /** Thông báo lỗi khi đổi mật khẩu không thành công hoặc không có quyền. */
  errorMessage: string | null = null;

  /** Thông báo thành công khi đổi mật khẩu thành công. */
  successMessage: string | null = null;

  /**
   * Khởi tạo AdminChangePasswordComponent với AuthService, UserAdminService, ActivatedRoute.
   * @param authService Dịch vụ xác thực để kiểm tra role Admin.
   * @param userAdminService Dịch vụ quản trị dùng để gửi yêu cầu đổi mật khẩu cho user mục tiêu.
   * @param route ActivatedRoute dùng để đọc route params và query params.
   */
  constructor(
    private authService: AuthService,
    private userAdminService: UserAdminService,
    private route: ActivatedRoute
  ) { }

  /**
   * Khi component khởi tạo:
   * - Kiểm tra quyền Admin.
   * - Đọc targetUserId từ route param và targetUserName từ query string.
   */
  ngOnInit(): void {
    if (!this.authService.isAdmin) {
      this.errorMessage = 'Bạn không có quyền đổi mật khẩu cho thành viên khác.';
      return;
    }

    this.targetUserId = this.route.snapshot.paramMap.get('userId');
    this.targetUserName = this.route.snapshot.queryParamMap.get('userName');

    if (!this.targetUserId) {
      this.errorMessage = 'Không xác định được thành viên cần đổi mật khẩu.';
    }
  }

  /**
   * Xử lý khi admin nhấn nút "Áp dụng".
   * Gửi yêu cầu đổi mật khẩu cho user mục tiêu tới API.
   */
  async onApply(): Promise<void> {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.targetUserId) {
      this.errorMessage = 'Không xác định được thành viên cần đổi mật khẩu.';
      return;
    }

    if (!this.newPassword) {
      this.errorMessage = 'Vui lòng nhập Mật khẩu mới.';
      return;
    }

    try {
      await this.userAdminService.changeMemberPassword(this.targetUserId, this.newPassword);
      this.successMessage = 'Đổi mật khẩu thành công.';
      this.newPassword = '';
    }
    catch (error: any) {
      this.errorMessage = error?.message ?? 'Đổi mật khẩu không thành công.';
    }
  }
}
