import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError, catchError } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  /**
   * Intercept (chặn) request HTTP để chuẩn hóa lỗi HTTP theo format business.
   * @param request HttpRequest đang được gửi đi.
   * @param next HttpHandler để chuyển request sang interceptor tiếp theo hoặc backend handler.
   * @returns Observable<HttpEvent> của pipeline HTTP.
   */
  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Xác định request có thuộc API của hệ thống hay không.
    const isApiRequest = this.isApiRequest(request.url);

    // Nếu không phải API request thì bỏ qua, không can thiệp.
    if (!isApiRequest) {
      return next.handle(request);
    }

    // Bắt lỗi và chuẩn hóa lỗi trước khi throw về downstream (Effects/Services).
    return next.handle(request).pipe(
      catchError((error: unknown) => {
        // Nếu không phải HttpErrorResponse thì throw nguyên trạng để không phá vỡ xử lý hiện có.
        if (!(error instanceof HttpErrorResponse)) {
          return throwError(() => error);
        }

        // Trích xuất message theo chuẩn business để UI hiển thị ổn định.
        const message = this.extractErrorMessage(error);

        // Chuẩn hóa body lỗi để đảm bảo có error.error.message.
        const normalizedBody = this.normalizeErrorBody(error.error, message);

        // Tạo HttpErrorResponse mới với body đã chuẩn hóa để downstream đọc được message thống nhất.
        const normalizedError = new HttpErrorResponse({
          error: normalizedBody,
          headers: error.headers,
          status: error.status,
          statusText: error.statusText,
          url: error.url ?? undefined,
        });

        return throwError(() => normalizedError);
      })
    );
  }

  /**
   * Xác định URL có thuộc API base URL của hệ thống hay không.
   * @param url URL của request hiện tại.
   * @returns true nếu thuộc API của hệ thống, ngược lại false.
   */
  private isApiRequest(url: string): boolean {
    // Chuẩn hóa baseUrl để tránh sai lệch do dấu '/' cuối.
    const baseUrl = this.normalizeBaseUrl(environment.apiBaseUrl);

    // Nếu baseUrl rỗng thì coi như không có phạm vi API để theo dõi.
    if (!baseUrl) {
      return false;
    }

    // Chuẩn hóa URL request để so khớp nhất quán.
    const requestUrl = this.normalizeBaseUrl(url);

    // Kiểm tra requestUrl có bắt đầu bằng baseUrl hay không.
    return requestUrl.startsWith(baseUrl);
  }

  /**
   * Chuẩn hóa baseUrl (normalize base URL) để tránh lỗi nối chuỗi khi có hoặc không có dấu '/' ở cuối.
   * @param baseUrl Base URL (đường dẫn gốc) cần chuẩn hóa.
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

    // Trường hợp baseUrl đã chuẩn: trả về 그대로.
    return baseUrl;
  }

  /**
   * Trích xuất message lỗi từ HttpErrorResponse để chuẩn hóa hiển thị.
   * @param error HttpErrorResponse phát sinh khi gọi API.
   * @returns Chuỗi message phù hợp để hiển thị.
   */
  private extractErrorMessage(error: HttpErrorResponse): string {
    // Ưu tiên đọc message từ body nếu backend trả dạng { message: "..." }.
    const errBody: unknown = error.error;

    if (errBody && typeof errBody === 'object' && 'message' in errBody) {
      const msg = (errBody as { message?: unknown }).message;

      if (typeof msg === 'string' && msg.trim().length > 0) {
        return msg;
      }
    }

    // Nếu là lỗi mạng (status=0) thì trả về thông điệp phù hợp theo ngữ cảnh.
    if (error.status === 0) {
      return 'Không thể kết nối đến server. Vui lòng kiểm tra mạng hoặc cấu hình API.';
    }

    // Fallback sang error.message do HttpClient cung cấp.
    if (typeof error.message === 'string' && error.message.trim().length > 0) {
      return error.message;
    }

    // Thông điệp mặc định khi không có dữ liệu message.
    return 'Yêu cầu không thành công. Vui lòng thử lại.';
  }

  /**
   * Chuẩn hóa error body để đảm bảo có cấu trúc { message: string } cho downstream.
   * @param body Body lỗi gốc từ backend hoặc từ HttpClient.
   * @param message Message đã trích xuất theo chuẩn business.
   * @returns Body lỗi đã chuẩn hóa.
   */
  private normalizeErrorBody(body: unknown, message: string): unknown {
    // Nếu body là object thì merge message vào để không mất thông tin gốc.
    if (body && typeof body === 'object') {
      // Nếu body đã có message thì giữ nguyên body gốc.
      if ('message' in body) {
        return body;
      }

      // Nếu body chưa có message thì bổ sung message.
      return {
        ...(body as Record<string, unknown>),
        message,
      };
    }

    // Nếu body không phải object thì tạo object mới có message.
    return {
      message,
    };
  }
}
