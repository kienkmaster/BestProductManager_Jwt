import { inject } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { Observable, catchError, from, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Interceptor xử lý HTTP request dùng chung.
 * Nhiệm vụ:
 * - Gửi kèm cookie xác thực (withCredentials đã được cấu hình ở từng request).
 * - Nếu gặp lỗi 401 (access token hết hạn), tự động gọi API refresh để lấy access token mới
 *   rồi retry lại request gốc một lần.
 */
let isRefreshing = false;

/**
 * Promise dùng chung để tránh gọi refresh đồng thời cho nhiều request 401.
 */
let refreshPromise: Promise<void> | null = null;

/**
 * Interceptor cho phép tự động xử lý luồng refresh token khi access token hết hạn.
 * @param req Request gốc gửi từ ứng dụng.
 * @param next Hàm delegate để chuyển request cho interceptor kế tiếp.
 * @returns Observable chứa HttpEvent từ pipeline HTTP.
 */
export const authTokenInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        const url = req.url ?? '';

        if (
          url.includes('/Account/login') ||
          url.includes('/Account/register') ||
          url.includes('/Account/refresh')
        ) {
          return throwError(() => error);
        }

        if (!isRefreshing) {
          isRefreshing = true;

          refreshPromise = authService.refreshAccessToken()
            .finally(() => {
              isRefreshing = false;
            });
        }

        return from(refreshPromise ?? Promise.resolve()).pipe(
          switchMap(() => next(req)),
          catchError(refreshError => {
            authService.clearLoginState();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
