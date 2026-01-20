import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

const ACCESS_TOKEN_STORAGE_KEY = 'best_product_manager_access_token';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  /**
   * Intercept (chặn) request HTTP để gắn JWT vào header Authorization theo chuẩn Bearer.
   * @param request HttpRequest đang được gửi đi.
   * @param next HttpHandler để chuyển request sang interceptor tiếp theo hoặc backend handler.
   * @returns Observable<HttpEvent> của pipeline HTTP.
   */
  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Chỉ áp dụng cho request gọi vào API base url của hệ thống.
    const isApiRequest = request.url.startsWith(environment.apiBaseUrl);

    if (!isApiRequest) {
      // Bỏ qua các request không thuộc API (assets, local resources...).
      return next.handle(request);
    }

    // Bỏ qua các endpoint không cần gắn token.
    const urlLower = request.url.toLowerCase();

    if (urlLower.includes('/account/login') || urlLower.includes('/account/register')) {
      return next.handle(request);
    }

    // Lấy token từ localStorage theo key chuẩn hóa của client.
    const token = this.getAccessToken();

    if (!token) {
      // Nếu chưa có token thì không thay đổi request.
      return next.handle(request);
    }

    // Clone request và gắn Authorization: Bearer <token>.
    const authorizedRequest = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });

    return next.handle(authorizedRequest);
  }

  /**
   * Đọc access token từ localStorage một cách an toàn.
   * @returns Token hoặc chuỗi rỗng khi không tồn tại/không truy cập được storage.
   */
  private getAccessToken(): string {
    // Bọc try/catch để tránh lỗi runtime trong các môi trường không hỗ trợ storage.
    try {
      const token = localStorage.getItem(ACCESS_TOKEN_STORAGE_KEY);

      if (!token) {
        return '';
      }

      return token.trim();
    } catch {
      return '';
    }
  }
}
