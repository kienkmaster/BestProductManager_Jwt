import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { API_ENDPOINTS } from './configs/api-endpoints';
import { LoginRequestModel } from '../../models/login-request.model';
import { AuthUserModel } from '../../models/auth-user.model';
import { AuthStateService } from '../state/auth-state.service';

type LoginApiRequest = {
  userName: string;
  password: string;
};

type LoginApiResponse = {
  userName: string;
  roles: string[];
};

type CurrentUserApiResponse = {
  userName: string;
  roles: string[];
};

@Injectable({
  providedIn: 'root',
})
export class UserAuthService {
  /**
   * Khởi tạo UserAuthService.
   * @param http HttpClient dùng để gọi API.
   * @param authStateService AuthStateService dùng để quản lý state.
   */
  constructor(
    private readonly http: HttpClient,
    private readonly authStateService: AuthStateService
  ) {}

  /**
   * Gọi API đăng nhập.
   * @param model Dữ liệu đăng nhập.
   * @returns Observable<AuthUserModel> chứa userName và roles khi đăng nhập thành công.
   */
  public loginAsync(model: LoginRequestModel): Observable<AuthUserModel> {
    // Resolve URL theo cấu hình API_ENDPOINTS và environment.
    const url = this.resolveApiUrl(API_ENDPOINTS.account.login);

    // Tạo request body theo contract của API login.
    const request: LoginApiRequest = {
      userName: model.userName,
      password: model.password,
    };

    // Gọi API login và map response về AuthUserModel để thống nhất model trong client.
    return this.http
      .post<LoginApiResponse>(url, request, {
        // Bật gửi/nhận cookie trong môi trường dev khác origin (nếu API dùng HttpOnly Cookie).
        withCredentials: true,
      })
      .pipe(
        map((response: LoginApiResponse) => {
          // Map response DTO sang model dùng trong client.
          return new AuthUserModel(response.userName, response.roles);
        })
      );
  }

  /**
   * Gọi API lấy thông tin user hiện tại (GET /Account/me).
   * @returns Observable<AuthUserModel> chứa userName và roles.
   */
  public getCurrentUserAsync(): Observable<AuthUserModel> {
    // Resolve URL theo cấu hình API_ENDPOINTS và environment.
    const url = this.resolveApiUrl(API_ENDPOINTS.account.me);

    // Gọi API lấy user hiện tại và map về AuthUserModel.
    return this.http
      .get<CurrentUserApiResponse>(url, {
        // Bật gửi cookie để server nhận access token từ HttpOnly Cookie (nếu áp dụng).
        withCredentials: true,
      })
      .pipe(
        map((response: CurrentUserApiResponse) => {
          // Map response DTO sang model dùng trong client.
          return new AuthUserModel(response.userName, response.roles);
        })
      );
  }

  /**
   * Thực hiện đăng nhập với quản lý state.
   * @param model Dữ liệu đăng nhập.
   */
  public login(model: LoginRequestModel): void {
    // Bật trạng thái đang submit.
    this.authStateService.loginSubmit();

    // Gọi API login.
    this.loginAsync(model).subscribe({
      next: (user: AuthUserModel) => {
        // Cập nhật state thành công.
        this.authStateService.loginSuccess(user);
      },
      error: (error: unknown) => {
        // Chuẩn hóa message lỗi và cập nhật state thất bại.
        const message = this.extractErrorMessage(error);
        this.authStateService.loginFailure(message);
      },
    });
  }

  /**
   * Thực hiện tải thông tin user hiện tại với quản lý state.
   */
  public loadCurrentUser(): void {
    // Bật trạng thái đang loading.
    this.authStateService.loadCurrentUser();

    // Gọi API lấy user hiện tại.
    this.getCurrentUserAsync().subscribe({
      next: (user: AuthUserModel) => {
        // Cập nhật state thành công.
        this.authStateService.loadCurrentUserSuccess(user);
      },
      error: (error: unknown) => {
        // Chuẩn hóa message lỗi và cập nhật state thất bại.
        const message = this.extractErrorMessage(error);
        this.authStateService.loadCurrentUserFailure(message);
      },
    });
  }

  /**
   * Resolve API URL: hỗ trợ cả absolute endpoint và relative endpoint.
   * @param endpoint Endpoint có thể là absolute URL hoặc relative path.
   * @returns Absolute URL để gọi HttpClient.
   */
  private resolveApiUrl(endpoint: string): string {
    // Nếu endpoint đã là absolute URL thì dùng trực tiếp.
    if (endpoint.startsWith('http://') || endpoint.startsWith('https://')) {
      return endpoint;
    }

    // Chuẩn hóa baseUrl để tránh lỗi nối chuỗi.
    const baseUrl = this.normalizeBaseUrl(environment.apiBaseUrl);

    // Nếu baseUrl rỗng thì trả về endpoint để tránh tạo URL sai.
    if (!baseUrl) {
      return endpoint;
    }

    // Ghép baseUrl + endpoint theo chuẩn.
    if (endpoint.startsWith('/')) {
      return `${baseUrl}${endpoint}`;
    }

    return `${baseUrl}/${endpoint}`;
  }

  /**
   * Chuẩn hóa baseUrl (normalize base URL) để tránh lỗi nối chuỗi khi có hoặc không có dấu '/' ở cuối.
   * @param baseUrl Base URL cần chuẩn hóa.
   * @returns Base URL đã chuẩn hóa (không có dấu '/' ở cuối).
   */
  private normalizeBaseUrl(baseUrl: string): string {
    // Trường hợp baseUrl rỗng: trả về rỗng để tránh tạo endpoint sai.
    if (!baseUrl) {
      return '';
    }

    // Trường hợp baseUrl kết thúc bằng '/': cắt bỏ ký tự '/' cuối để thống nhất.
    if (baseUrl.endsWith('/')) {
      return baseUrl.substring(0, baseUrl.length - 1);
    }

    // Trường hợp baseUrl đã chuẩn: trả về nguyên gốc.
    return baseUrl;
  }

  /**
   * Chuẩn hóa message lỗi (normalize error message) để UI sử dụng ổn định.
   * @param error Lỗi nhận từ HttpClient hoặc runtime.
   * @returns Message lỗi đã chuẩn hóa.
   */
  private extractErrorMessage(error: unknown): string {
    // Trường hợp HttpErrorResponse: ưu tiên error.error.message.
    if (error instanceof HttpErrorResponse) {
      // Ưu tiên đọc message theo chuẩn { message: "..." }.
      const body = error.error as unknown;

      if (body && typeof body === 'object' && 'message' in body) {
        const msg = (body as { message?: unknown }).message;

        if (typeof msg === 'string' && msg.trim().length > 0) {
          return msg;
        }
      }

      // Trường hợp lỗi mạng (status = 0).
      if (error.status === 0) {
        return 'Không thể kết nối đến server. Vui lòng kiểm tra mạng hoặc cấu hình API.';
      }

      // Fallback theo message mặc định của HttpClient.
      if (typeof error.message === 'string' && error.message.trim().length > 0) {
        return error.message;
      }
    }

    // Trường hợp lỗi không xác định.
    return 'Yêu cầu không thành công. Vui lòng thử lại.';
  }
}
