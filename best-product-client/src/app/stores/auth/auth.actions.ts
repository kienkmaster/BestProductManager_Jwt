import { createAction, props } from '@ngrx/store';

import { AuthUserModel } from '../../models/auth-user.model';

/**
 * Reset status (message/error) dùng chung cho feature Auth.
 */
export const authResetStatus = createAction('[Auth] Reset Status');

/**
 * Ghi nhận lỗi validate phía client (client validation) trong luồng login.
 */
export const authClientValidationFailed = createAction(
  '[Auth] Client Validation Failed',
  props<{ message: string }>()
);

/**
 * Submit login (bắt đầu đăng nhập).
 */
export const authLoginSubmit = createAction(
  '[Auth] Login Submit',
  props<{ model: unknown }>()
);

/**
 * Login thành công (payload user).
 */
export const authLoginSuccess = createAction(
  '[Auth] Login Success',
  props<{ user: AuthUserModel }>()
);

/**
 * Login thất bại (payload message).
 */
export const authLoginFailure = createAction(
  '[Auth] Login Failure',
  props<{ message: string }>()
);

/**
 * Đánh dấu đã xử lý điều hướng về trang chủ sau login.
 */
export const authNavigateHomeHandled = createAction('[Auth] Navigate Home Handled');

/**
 * Logout.
 */
export const authLogout = createAction('[Auth] Logout');

/**
 * User click link tên user để yêu cầu tải thông tin user hiện tại (GET /Account/me).
 */
export const authLoadCurrentUser = createAction('[Auth] Load Current User');

/**
 * Tải thông tin user hiện tại thành công.
 */
export const authLoadCurrentUserSuccess = createAction(
  '[Auth] Load Current User Success',
  props<{ user: AuthUserModel }>()
);

/**
 * Tải thông tin user hiện tại thất bại.
 */
export const authLoadCurrentUserFailure = createAction(
  '[Auth] Load Current User Failure',
  props<{ message: string }>()
);
