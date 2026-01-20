import { createAction, props } from '@ngrx/store';

import { RegisterUserModel } from '../../models/register-user.model';

/**
 * Reset trạng thái status (status state) của đăng ký user.
 * Mục tiêu: xóa status cũ trước khi thực hiện submit mới.
 */
export const userRegisterResetStatus = createAction(
  '[User Register] Reset Status'
);

/**
 * Báo lỗi validate phía client (client-side validation) khi submit form không hợp lệ.
 * @param message Thông điệp lỗi cần hiển thị.
 */
export const userRegisterClientValidationFailed = createAction(
  '[User Register] Client Validation Failed',
  // Action này có một thuộc tính tên là message kiểu string.
  props<{ message: string }>()
);

/**
 * Submit đăng ký user (submit register).
 * @param model Dữ liệu đăng ký (DTO-like) gửi API.
 */
export const userRegisterSubmit = createAction(
  '[User Register] Submit',
  props<{ model: RegisterUserModel }>()
);

/**
 * Đăng ký thành công (register success).
 * @param message Thông điệp thành công từ backend hoặc message mặc định.
 * @param userName Tên user vừa đăng ký thành công.
 */
export const userRegisterSuccess = createAction(
  '[User Register] Success',
  props<{ message: string; userName: string }>()
);

/**
 * Đăng ký thất bại (register failure).
 * @param message Thông điệp lỗi từ backend hoặc message mặc định.
 */
export const userRegisterFailure = createAction(
  '[User Register] Failure',
  props<{ message: string }>()
);

/**
 * Xác nhận đã xử lý reset form (reset handled) sau khi store yêu cầu reset.
 * Mục tiêu: tránh lặp reset form khi state vẫn đang giữ cờ reset.
 */
export const userRegisterResetFormHandled = createAction(
  '[User Register] Reset Form Handled'
);
