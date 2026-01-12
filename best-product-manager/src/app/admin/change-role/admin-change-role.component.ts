import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, NgForOf } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserAdminService } from '../../core/services/user-admin.service';
import { RoleOption } from '../../shared/models/role.model';
import { UserRoleInfo } from '../../shared/models/user-role-info.model';

/**
 * Màn hình Thay đổi phân loại thành viên.
 * Hiển thị:
 * - Tên thành viên (textbox chỉ đọc).
 * - Combobox Phân loại (danh sách role).
 * - Nút "Cập nhật" để lưu thay đổi role cho user.
 */
@Component({
  selector: 'app-admin-change-role',
  standalone: true,
  imports: [
    FormsModule,
    NgIf,
    NgForOf
  ],
  templateUrl: './admin-change-role.component.html',
  styleUrl: './admin-change-role.component.css'
})
export class AdminChangeRoleComponent implements OnInit {
  /** Id của user mục tiêu được lấy từ route parameter. */
  targetUserId: string | null = null;

  /** Tên đăng nhập của user mục tiêu (lấy từ query string để hiển thị). */
  targetUserName: string | null = null;

  /** Danh sách role dùng cho combobox Phân loại. */
  roles: RoleOption[] = [];

  /** Id của role đang được chọn trong combobox. */
  selectedRoleId: string | null = null;

  /** Thông báo lỗi khi tải dữ liệu hoặc cập nhật không thành công. */
  errorMessage: string | null = null;

  /** Thông báo thành công khi cập nhật phân loại thành công. */
  successMessage: string | null = null;

  /**
   * Khởi tạo AdminChangeRoleComponent với AuthService, UserAdminService và ActivatedRoute.
   * @param authService Dịch vụ xác thực để kiểm tra role Admin.
   * @param userAdminService Dịch vụ quản trị dùng để lấy và cập nhật phân loại thành viên.
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
   * - Tải danh sách role và phân loại hiện tại của user.
   */
  async ngOnInit(): Promise<void> {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.authService.isAdmin) {
      this.errorMessage = 'Bạn không có quyền thay đổi phân loại thành viên.';
      return;
    }

    this.targetUserId = this.route.snapshot.paramMap.get('userId');
    this.targetUserName = this.route.snapshot.queryParamMap.get('userName');

    if (!this.targetUserId) {
      this.errorMessage = 'Không xác định được thành viên cần thay đổi phân loại.';
      return;
    }

    try {
      const roles = await this.userAdminService.getAllRoles();
      this.roles = roles;

      const userRoleInfo: UserRoleInfo = await this.userAdminService.getUserRole(this.targetUserId);
      this.selectedRoleId = userRoleInfo.roleId ?? null;
    }
    catch (error: any) {
      this.errorMessage = error?.message ?? 'Không thể tải dữ liệu phân loại thành viên từ API.';
    }
  }

  /**
   * Xử lý khi admin nhấn nút "Cập nhật".
   * Gửi yêu cầu cập nhật role mới cho user mục tiêu tới API.
   */
  async onApply(): Promise<void> {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.targetUserId) {
      this.errorMessage = 'Không xác định được thành viên cần thay đổi phân loại.';
      return;
    }

    if (!this.selectedRoleId) {
      this.errorMessage = 'Vui lòng chọn Phân loại cần cập nhật.';
      return;
    }

    try {
      await this.userAdminService.updateUserRole(this.targetUserId, this.selectedRoleId);
      this.successMessage = 'Cập nhật phân loại thành công.';
    }
    catch (error: any) {
      this.errorMessage = error?.message ?? 'Cập nhật phân loại không thành công.';
    }
  }
}
