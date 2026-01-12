import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authTokenInterceptor } from './core/interceptors/auth-token.interceptor';

/**
 * Cấu hình gốc cho ứng dụng Angular:
 * - Đăng ký hệ thống định tuyến (routing).
 * - Đăng ký HttpClient và interceptor gắn JWT token.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        authTokenInterceptor
      ])
    )
  ]
};
