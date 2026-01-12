import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

/**
 * Dịch vụ xác thực người dùng (authentication service).
 * Sử dụng JWT lưu trong HttpOnly Cookie để gọi các API tài khoản của BestProductManager.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  /**
   * Địa chỉ gốc của API BestProductManager.
   */
  // private readonly apiBaseUrl = 'https://localhost:7130/api';
  private readonly apiBaseUrl = `http://localhost/${environment.ApiTarget}/api`;

  /** Cờ trạng thái cho biết người dùng đã đăng nhập hay chưa (theo phía frontend). */
  private _isLoggedIn = false;

  /** Tên người dùng hiện tại (nếu đã đăng nhập). */
  private _userName: string | null = null;

  /** Danh sách role hiện tại của user (ví dụ: admin, user, partner). */
  private _roles: string[] = [];

  /**
   * Khởi tạo AuthService với HttpClient.
   * @param http HttpClient dùng để gọi API.
   */
  constructor(private http: HttpClient) { }

  /**
   * Thuộc tính chỉ đọc cho biết người dùng đã đăng nhập hay chưa.
   */
  get isLoggedIn(): boolean {
    return this._isLoggedIn;
  }

  /**
   * Lấy tên người dùng hiện tại (nếu đã đăng nhập).
   */
  get userName(): string | null {
    return this._userName;
  }

  /**
   * Lấy danh sách role hiện tại của user.
   */
  get roles(): string[] {
    return this._roles;
  }

  /**
   * Cho biết user hiện tại có thuộc role Admin hay không.
   * Dùng để điều khiển hiển thị các menu, màn hình quản trị.
   */
  get isAdmin(): boolean {
    return this._roles.some(role => role?.toLowerCase() === 'admin');
  }

  /**
   * Gọi API đăng nhập và thiết lập trạng thái đăng nhập trong frontend.
   * Sau khi đăng nhập thành công, server trả về HttpOnly Cookie chứa JWT và body trả về userName + roles.
   * @param userName Tên đăng nhập.
   * @param password Mật khẩu.
   */
  async login(userName: string, password: string): Promise<void> {
    const trimmedName = userName?.trim();

    if (!trimmedName || !password) {
      throw new Error('Tên đăng nhập và mật khẩu là bắt buộc.');
    }

    try {
      const result = await firstValueFrom(
        this.http.post<{ userName?: string | null; roles?: string[] }>(
          `${this.apiBaseUrl}/Account/login`,
          {
            userName: trimmedName,
            password: password
          },
          {
            withCredentials: true
          }
        )
      );

      this._isLoggedIn = true;
      this._userName = result.userName ?? trimmedName;
      this._roles = result.roles ?? [];
    }
    catch (error: any) {
      this.clearLoginState();

      const message =
        error?.error?.message
        ?? error?.message
        ?? 'Tên đăng nhập hoặc mật khẩu không hợp lệ.';

      throw new Error(message);
    }
  }

  /**
   * Gọi API logout để xóa HttpOnly Cookie chứa JWT và reset trạng thái đăng nhập phía client.
   */
  async logout(): Promise<void> {
    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/Account/logout`,
          {},
          {
            withCredentials: true
          }
        )
      );
    }
    finally {
      this.clearLoginState();
    }
  }

  /**
   * Gọi API register để đăng ký người dùng mới.
   * Dùng DTO tương ứng với RegisterUserDto trên backend.
   * @param userName Tên đăng nhập.
   * @param password Mật khẩu.
   * @param confirmPassword Mật khẩu xác nhận.
   * @returns true nếu đăng ký thành công.
   */
  async register(userName: string, password: string, confirmPassword: string): Promise<boolean> {
    const trimmedName = userName?.trim();

    if (!trimmedName || !password || !confirmPassword) {
      throw new Error('Vui lòng nhập đầy đủ Tên đăng nhập, Mật khẩu và Xác nhận mật khẩu.');
    }

    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/Account/register`,
          {
            userName: trimmedName,
            password: password,
            confirmPassword: confirmPassword
          },
          {
            withCredentials: true
          }
        )
      );

      return true;
    }
    catch (error: any) {
      const message =
        error?.error?.message
        ?? error?.message
        ?? 'Đăng ký tài khoản không thành công.';

      throw new Error(message);
    }
  }

  /**
   * Gọi API /account/me để kiểm tra trạng thái đăng nhập hiện tại dựa trên JWT trong HttpOnly Cookie.
   * API này trả về userName và danh sách roles.
   */
  async refreshLoginStatusFromApi(): Promise<void> {
    try {
      const result = await firstValueFrom(
        this.http.get<{ userName?: string | null; roles?: string[] }>(
          `${this.apiBaseUrl}/Account/me`,
          {
            withCredentials: true
          }
        )
      );

      if (result?.userName) {
        this._isLoggedIn = true;
        this._userName = result.userName;
        this._roles = result.roles ?? [];
      }
      else {
        this.clearLoginState();
      }
    }
    catch {
      this.clearLoginState();
    }
  }

  /**
   * Gọi API đổi mật khẩu cho chính user hiện tại.
   * Rule validation phía server sử dụng cùng rule với đăng ký user.
   * @param currentPassword Mật khẩu hiện tại.
   * @param newPassword Mật khẩu mới.
   * @param confirmNewPassword Xác nhận mật khẩu mới.
   */
  async changePassword(currentPassword: string, newPassword: string, confirmNewPassword: string): Promise<void> {
    const trimmedCurrent = currentPassword ?? '';
    const trimmedNew = newPassword ?? '';
    const trimmedConfirm = confirmNewPassword ?? '';

    if (!trimmedCurrent || !trimmedNew || !trimmedConfirm) {
      throw new Error('Vui lòng nhập đầy đủ Mật khẩu cũ, Mật khẩu mới và Xác nhận mật khẩu mới.');
    }

    if (trimmedNew !== trimmedConfirm) {
      throw new Error('Mật khẩu mới và Xác nhận mật khẩu mới không khớp.');
    }

    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/Account/change-password`,
          {
            currentPassword: trimmedCurrent,
            newPassword: trimmedNew,
            confirmNewPassword: trimmedConfirm
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
        ?? 'Đổi mật khẩu không thành công.';

      throw new Error(message);
    }
  }

  /**
   * Gọi API /account/refresh để xin access token mới dựa trên refresh token trong HttpOnly Cookie.
   * Sau khi refresh thành công, cập nhật lại trạng thái đăng nhập hiện tại.
   */
  async refreshAccessToken(): Promise<void> {
    try {
      await firstValueFrom(
        this.http.post<{ message?: string }>(
          `${this.apiBaseUrl}/Account/refresh`,
          {},
          {
            withCredentials: true
          }
        )
      );

      await this.refreshLoginStatusFromApi();
    }
    catch (error: any) {
      this.clearLoginState();

      const message =
        error?.error?.message
        ?? error?.message
        ?? 'Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.';

      throw new Error(message);
    }
  }

  /**
   * Reset trạng thái đăng nhập phía frontend về mặc định.
   * Dùng trong các trường hợp logout hoặc refresh token thất bại.
   */
  clearLoginState(): void {
    this._isLoggedIn = false;
    this._userName = null;
    this._roles = [];
  }
}
