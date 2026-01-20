import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { environment } from '../../../../environments/environment';
import { loadingRequestEnded, loadingRequestStarted } from '../../../stores/loading/loading.actions';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  /**
   * Khởi tạo LoadingInterceptor.
   * @param store NgRx Store dùng để dispatch action bật/tắt loading.
   */
  constructor(private readonly store: Store) { }

  /**
   * Intercept (chặn) request HTTP để bật/tắt loading theo request API.
   * @param request HttpRequest đang được gửi đi.
   * @param next HttpHandler để chuyển request sang interceptor tiếp theo hoặc backend handler.
   * @returns Observable<HttpEvent> của pipeline HTTP.
   */
  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Chỉ áp dụng loading cho request gọi đến API base url của hệ thống.
    const shouldTrack = request.url.startsWith(environment.apiBaseUrl);

    if (!shouldTrack) {
      // Bỏ qua các request không thuộc API (ví dụ assets, local resources).
      return next.handle(request);
    }

    // Dispatch action bắt đầu request để bật spinner.
    this.store.dispatch(loadingRequestStarted());

    // Kết thúc request (thành công hoặc lỗi) đều phải dispatch action kết thúc để tắt spinner.
    return next.handle(request).pipe(
      finalize(() => {
        // Dispatch action kết thúc request để giảm activeRequests.
        this.store.dispatch(loadingRequestEnded());
      })
    );
  }
}
