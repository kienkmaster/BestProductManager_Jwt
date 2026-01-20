import { Injectable, inject } from '@angular/core';

import { HttpErrorResponse } from '@angular/common/http';

import { Actions, createEffect, ofType } from '@ngrx/effects';

import { exhaustMap, map, catchError, of } from 'rxjs';

import { UserRegisterService } from '../../services/core/user-register.service';

import { ApiMessageResponseModel } from '../../models/api-message-response.model';

import { userRegisterFailure, userRegisterSubmit, userRegisterSuccess } from './user-register.actions';

@Injectable()
export class UserRegisterEffects {
  /**
   * Stream actions của NgRx (Actions stream).
   */
  private readonly actions$ = inject(Actions);

  /**
   * Service gọi API đăng ký user.
   */
  private readonly userRegisterService = inject(UserRegisterService);

  /**
   * Effect xử lý submit đăng ký user.
   * - Nhận action submit
   * - Gọi API đăng ký
   * - Thành công: dispatch success
   * - Thất bại: dispatch failure
   */
  public readonly submitRegister$ = createEffect(() => {
    // Lắng nghe action submit của đăng ký user.
    return this.actions$.pipe(
      ofType(userRegisterSubmit),

      // Dùng exhaustMap để tránh gửi nhiều request song song khi user click liên tiếp.
      exhaustMap((action) => {
        // Gọi API đăng ký thông qua service.
        return this.userRegisterService.registerAsync(action.model).pipe(
          map((response: ApiMessageResponseModel) => {
            // Chuẩn hóa message thành công để UI hiển thị ổn định.
            const successMessage = response?.message || 'Đăng ký tài khoản thành công.';

            // Dispatch action success kèm message và userName.
            return userRegisterSuccess({
              message: successMessage,
              userName: action.model.userName,
            });
          }),
          catchError((error: unknown) => {
            // Trích xuất message lỗi theo chuẩn business.
            const message = this.extractErrorMessage(error);

            // Dispatch action failure kèm message lỗi.
            return of(userRegisterFailure({ message }));
          })
        );
      })
    );
  });

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
