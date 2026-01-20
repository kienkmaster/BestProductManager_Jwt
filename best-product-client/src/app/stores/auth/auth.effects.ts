import { Injectable, inject } from '@angular/core';

import { HttpErrorResponse } from '@angular/common/http';

import { Actions, createEffect, ofType } from '@ngrx/effects';

import { Store } from '@ngrx/store';

import { catchError, exhaustMap, map, of, switchMap } from 'rxjs';

import { UserAuthService } from '../../services/core/user-auth.service';

import {
  authLoadCurrentUser,
  authLoadCurrentUserFailure,
  authLoadCurrentUserSuccess,
  authLoginFailure,
  authLoginSubmit,
  authLoginSuccess,
} from './auth.actions';

import { AuthUserModel } from '../../models/auth-user.model';

@Injectable()
export class AuthEffects {
  /**
   * Actions stream (luồng action) của NgRx.
   */
  private readonly actions$ = inject(Actions);

  /**
   * Service gọi API cho auth (login/me).
   */
  private readonly userAuthService = inject(UserAuthService);

  /**
   * Store dùng cho các tình huống cần select/dispatch bổ sung (mở rộng sau).
   */
  private readonly store = inject(Store);

  /**
   * Effect xử lý login: bắt authLoginSubmit → gọi API login → dispatch success/failure.
   * + - exhaustMap: Bỏ qua request mới khi đang xử lý	Phù hợp nhất
   * x - mergeMap: Xử lý song song	(Không phù hợp vì spam login)
   * x - switchMap: Hủy request cũ, giữ request mới	(Không phù hợp vì không được hủy login)
   * x - concatMap: Xếp hàng xử lý tuần tự (Không phù hợp spam login)
   */
  public readonly login$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(authLoginSubmit),
      exhaustMap((action: { model: unknown }) => {
        // Gọi API login thông qua service.
        return this.userAuthService.loginAsync(action.model).pipe(
          map((user: AuthUserModel) => {
            // Dispatch action login success để reducer cập nhật state.
            return authLoginSuccess({ user });
          }),
          catchError((error: unknown) => {
            // Chuẩn hóa message lỗi để UI hiển thị nhất quán.
            const message = this.extractErrorMessage(error);

            // Dispatch action login failure để reducer cập nhật state lỗi.
            return of(authLoginFailure({ message }));
          })
        );
      })
    );
  });

  /**
   * Effect tải current user: bắt authLoadCurrentUser → gọi API /Account/me → dispatch success/failure.
   */
  public readonly loadCurrentUser$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(authLoadCurrentUser),
      exhaustMap(() => {
        // Gọi API lấy thông tin user hiện tại thông qua service.
        return this.userAuthService.getCurrentUserAsync().pipe(
          map((user: AuthUserModel) => {
            // Dispatch action load current user success để reducer lưu dữ liệu.
            return authLoadCurrentUserSuccess({ user });
          }),
          catchError((error: unknown) => {
            // Chuẩn hóa message lỗi để UI hiển thị nhất quán.
            const message = this.extractErrorMessage(error);

            // Dispatch action load current user failure để reducer lưu message lỗi.
            return of(authLoadCurrentUserFailure({ message }));
          })
        );
      })
    );
  });

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
