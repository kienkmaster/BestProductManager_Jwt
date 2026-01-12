import { Component, OnInit } from '@angular/core';
import { NgIf, NgForOf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserAdminService } from '../../core/services/user-admin.service';
import { MemberSummary } from '../../shared/models/member-summary.model';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình Quản lý thành viên (chỉ dành cho Admin).
 * Hiển thị danh sách user với phân loại:
 * - Thông thường
 * - Đối tác (Partner)
 * - Quản lý (Admin)
 * Và cung cấp các link thao tác đổi mật khẩu và thay đổi phân loại cho từng user.
 */
@Component({
  selector: 'app-member-management',
  standalone: true,
  imports: [
    NgIf,
    NgForOf,
    RouterLink
  ],
  templateUrl: './member-management.component.html',
  styleUrl: './member-management.component.css'
})
export class MemberManagementComponent implements OnInit {
  /** Danh sách thành viên hiển thị trên bảng. */
  members: MemberSummary[] = [];

  /** Thông báo lỗi khi tải danh sách hoặc khi không có quyền. */
  errorMessage: string | null = null;

  /**
   * Khởi tạo MemberManagementComponent với UserAdminService, AuthService.
   * @param userAdminService Dịch vụ quản trị dùng để gọi API danh sách user.
   * @param authService Dịch vụ xác thực để kiểm tra role Admin.
   */
  constructor(
    private userAdminService: UserAdminService,
    private authService: AuthService
  ) { }

  /**
   * Khi component khởi tạo:
   * - Nếu user không phải Admin: hiển thị thông báo không có quyền.
   * - Nếu là Admin: gọi API để lấy danh sách thành viên.
   */
  async ngOnInit(): Promise<void> {
    this.errorMessage = null;
    this.members = [];

    if (!this.authService.isAdmin) {
      this.errorMessage = 'Bạn không có quyền quản lý thành viên.';
      return;
    }

    try {
      this.members = await this.userAdminService.getAllMembers();
    }
    catch {
      this.errorMessage = 'Không thể tải danh sách thành viên từ API.';
    }
  }

  /**
   * Cho biết user tương ứng có được phân loại là Quản lý (Admin) hay không.
   * @param member Thành viên cần kiểm tra.
   */
  isMemberAdmin(member: MemberSummary): boolean {
    return member.isAdmin;
  }

  /**
   * Cho biết user tương ứng có được phân loại là Đối tác (Partner) hay không.
   * @param member Thành viên cần kiểm tra.
   */
  isMemberPartner(member: MemberSummary): boolean {
    const roleName = member.roleName?.toLowerCase() ?? '';
    return roleName === 'partner';
  }

  /**
   * Cho biết user tương ứng có được phân loại là Thông thường (không phải Admin, không phải Đối tác) hay không.
   * @param member Thành viên cần kiểm tra.
   */
  isMemberNormal(member: MemberSummary): boolean {
    const roleName = member.roleName?.toLowerCase() ?? '';

    if (roleName === 'partner') {
      return false;
    }

    return !member.isAdmin;
  }
}
