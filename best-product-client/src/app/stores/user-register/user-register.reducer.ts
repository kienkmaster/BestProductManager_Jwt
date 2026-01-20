import { createReducer, on } from '@ngrx/store';

import {
  userRegisterClientValidationFailed,
  userRegisterFailure,
  userRegisterResetFormHandled,
  userRegisterResetStatus,
  userRegisterSubmit,
  userRegisterSuccess,
} from './user-register.actions';

import { initialUserRegisterState, UserRegisterState } from './user-register.store';

/**
 * Reducer của đăng ký user (User Register Reducer).
 * Mục tiêu: quản lý trạng thái UI liên quan đăng ký (spinner/status/reset flag).
 */
export const userRegisterReducer = createReducer<UserRegisterState>(
  initialUserRegisterState,

  on(userRegisterResetStatus, (state: UserRegisterState) => {
    /**
     * Reset status để xóa thông điệp cũ trước khi submit mới.
     */
    return {
      ...state,
      statusMessage: '',
      isErrorStatus: false,
    };
  }),

  on(userRegisterClientValidationFailed, (state: UserRegisterState, action) => {
    /**
     * Gán lỗi validate phía client để UI hiển thị.
     */
    return {
      ...state,
      statusMessage: action.message,
      isErrorStatus: true,
      isSubmitting: false,
    };
  }),

  on(userRegisterSubmit, (state: UserRegisterState) => {
    /**
     * Bật submitting để UI hiển thị spinner và khóa submit.
     * Đồng thời reset status cũ và hạ cờ reset form.
     */
    return {
      ...state,
      isSubmitting: true,
      statusMessage: '',
      isErrorStatus: false,
      shouldResetForm: false,
    };
  }),

  on(userRegisterSuccess, (state: UserRegisterState, action) => {
    /**
     * Thành công: tắt submitting, set message thành công, bật cờ reset form.
     */
    return {
      ...state,
      isSubmitting: false,
      statusMessage: action.message,
      isErrorStatus: false,
      shouldResetForm: true,
      lastRegisteredUserName: action.userName,
    };
  }),

  on(userRegisterFailure, (state: UserRegisterState, action) => {
    /**
     * Thất bại: tắt submitting và hiển thị message lỗi.
     */
    return {
      ...state,
      isSubmitting: false,
      statusMessage: action.message,
      isErrorStatus: true,
      shouldResetForm: false,
    };
  }),

  on(userRegisterResetFormHandled, (state: UserRegisterState) => {
    /**
     * Component đã reset form xong: hạ cờ để tránh reset lặp.
     */
    return {
      ...state,
      shouldResetForm: false,
    };
  }),
);
