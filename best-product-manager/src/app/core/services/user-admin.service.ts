import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { MemberSummary } from '../../shared/models/member-summary.model';
import { RoleOption } from '../../shared/models/role.model';
import { UserRoleInfo } from '../../shared/models/user-role-info.model';
import { environment } from '../../../environments/environment';

/**
 * Dịch vụ quản trị thành viên (chỉ dùng cho Admin).
 * Chịu trách nhiệm:
 * - Lấy danh sách thành viên.
 * - Đổi mật khẩu cho thành viên khác (reset mật khẩu).
 * - Quản lý phân loại (role) thành viên.
 */
@Injectable({
  providedIn: 'root'
})
export class UserAdminService {
  /**
   * Địa chỉ gốc của API BestProductManager.
   * Dùng cho các API quản trị liên quan đến thành viên.
   */
  // private readonly apiBaseUrl = 'https://localhost:7130/api';
  private readonly apiBaseUrl = `http://localhost/${environment.ApiTarget}/api`;

  /**
   * Khởi tạo UserAdminService với HttpClient.
   * @param http HttpClient dùng để gọi API.
   */
  constructor(private http: HttpClient) { }

  /**
   * Lấy danh sách tất cả thành viên (chỉ Admin mới có quyền gọi).
   * @returns Danh sách MemberSummary từ API.
   */
  async getAllMembers(): Promise<MemberSummary[]> {
    const members = await firstValueFrom(
      this.http.get<MemberSummary[]>(
        `${this.apiBaseUrl}/admin/users`,
        {
          withCredentials: true
        }
      )
    );

    return members;
  }

  /**
   * Đổi mật khẩu cho một thành viên cụ thể, do Admin thực hiện.
   * @param userId Id của user cần đổi mật khẩu.
   * @param newPassword Mật khẩu mới do admin đặt.
   */
  async changeMemberPassword(userId: string, newPassword: string): Promise<void> {
    const trimmedPassword = newPassword ?? '';

    if (!trimmedPassword) {
      throw new Error('Vui lòng nhập Mật khẩu mới.');
    }

    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/admin/users/${encodeURIComponent(userId)}/change-password`,
          {
            newPassword: trimmedPassword
          },
          {
            withCredentials: true
          }
        )
      );
    }
    catch (error: any) {
      const message =
        error?.error?.message
        ?? error?.message
        ?? 'Đổi mật khẩu cho thành viên không thành công.';

      throw new Error(message);
    }
  }

  /**
   * Lấy danh sách toàn bộ role trong hệ thống dùng cho combobox phân loại.
   * @returns Danh sách RoleOption từ API.
   */
  async getAllRoles(): Promise<RoleOption[]> {
    const roles = await firstValueFrom(
      this.http.get<RoleOption[]>(
        `${this.apiBaseUrl}/admin/users/roles`,
        {
          withCredentials: true
        }
      )
    );

    return roles;
  }

  /**
   * Lấy thông tin phân loại role hiện tại của một user.
   * @param userId Id của user cần tra cứu.
   * @returns Thông tin UserRoleInfo từ API.
   */
  async getUserRole(userId: string): Promise<UserRoleInfo> {
    const info = await firstValueFrom(
      this.http.get<UserRoleInfo>(
        `${this.apiBaseUrl}/admin/users/${encodeURIComponent(userId)}/role`,
        {
          withCredentials: true
        }
      )
    );

    return info;
  }

  /**
   * Cập nhật phân loại (role) chính cho một user.
   * @param userId Id của user cần cập nhật.
   * @param roleId Id của role mới cần gán cho user.
   */
  async updateUserRole(userId: string, roleId: string): Promise<void> {
    const trimmedRoleId = roleId ?? '';

    if (!trimmedRoleId) {
      throw new Error('Vui lòng chọn Phân loại cần cập nhật.');
    }

    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/admin/users/${encodeURIComponent(userId)}/role`,
          {
            roleId: trimmedRoleId
          },
          {
            withCredentials: true
          }
        )
      );
    }
    catch (error: any) {
      const message =
        error?.error?.message
        ?? error?.message
        ?? 'Cập nhật phân loại không thành công.';

      throw new Error(message);
    }
  }
}
