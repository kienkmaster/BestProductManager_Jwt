import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterUserModel } from '../../models/register-user.model';
import { ApiMessageResponseModel } from '../../models/api-message-response.model';
import { API_ENDPOINTS } from './configs/api-endpoints';
import { environment } from '../../../environments/environment';
import { UserRegisterStateService } from '../state/user-register-state.service';

@Injectable({
  /**
   * Cấu hình provider theo root injector để service dùng toàn ứng dụng.
   */
  providedIn: 'root',
})
export class UserRegisterService {
  /**
   * Khởi tạo service đăng ký (UserRegisterService).
   * @param http HttpClient (giao tiếp HTTP) dùng để gọi API.
   * @param userRegisterStateService UserRegisterStateService dùng để quản lý state.
   */
  constructor(
    private readonly http: HttpClient,
    private readonly userRegisterStateService: UserRegisterStateService
  ) {
    /**
     * Service tập trung xử lý gọi API, không chứa UI logic.
     */
  }

  /**
   * Gọi API đăng ký user (register user).
   * @param model RegisterUserModel chứa userName/password/confirmPassword.
   * @returns Observable<ApiMessageResponseModel> để component subscribe và xử lý kết quả.
   */
  public registerAsync(model: RegisterUserModel): Observable<ApiMessageResponseModel> {
    // Resolve URL theo cấu hình API_ENDPOINTS và environment.
    const url = this.resolveApiUrl(API_ENDPOINTS.account.register);

    /**
     * Thực hiện POST (HTTP) tới endpoint đăng ký đã quản lý tập trung.
     */
    return this.http.post<ApiMessageResponseModel>(
      url,
      model,
      {
        withCredentials: true,
      }
    );
  }

  /**
   * Thực hiện đăng ký với quản lý state.
   * @param model RegisterUserModel chứa userName/password/confirmPassword.
   */
  public register(model: RegisterUserModel): void {
    // Bật trạng thái đang submit.
    this.userRegisterStateService.submit();

    // Gọi API đăng ký.
    this.registerAsync(model).subscribe({
      next: (response: ApiMessageResponseModel) => {
        // Chuẩn hóa message thành công để UI hiển thị ổn định.
        const successMessage = response?.message || 'Đăng ký tài khoản thành công.';

        // Cập nhật state thành công.
        this.userRegisterStateService.success(successMessage, model.userName);
      },
      error: (error: unknown) => {
        // Chuẩn hóa message lỗi và cập nhật state thất bại.
        const message = this.extractErrorMessage(error);
        this.userRegisterStateService.failure(message);
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
   * Trích xuất message lỗi từ lỗi HTTP để hiển thị UI.
   * @param error Lỗi phát sinh khi gọi API.
   * @returns Chuỗi message phù hợp để hiển thị.
   */
  private extractErrorMessage(error: unknown): string {
    // Nếu không phải HttpErrorResponse: trả về thông điệp lỗi chung.
    if (!(error instanceof HttpErrorResponse)) {
      return 'Đăng ký không thành công. Vui lòng thử lại.';
    }

    // Ưu tiên đọc message từ body nếu backend trả về dạng { message: "..." }.
    const errBody: unknown = error.error;

    if (errBody && typeof errBody === 'object' && 'message' in errBody) {
      const msg = (errBody as { message?: unknown }).message;

      if (typeof msg === 'string' && msg.trim().length > 0) {
        return msg;
      }
    }

    // Fallback sang error.message do HttpClient cung cấp.
    if (typeof error.message === 'string' && error.message.trim().length > 0) {
      return error.message;
    }

    // Thông điệp mặc định khi không có dữ liệu message.
    return 'Đăng ký không thành công. Vui lòng thử lại.';
  }
}
