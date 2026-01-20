import { createReducer, on } from '@ngrx/store';

import { initialAuthState } from './auth.store';

import {
  authClientValidationFailed,
  authLoadCurrentUser,
  authLoadCurrentUserFailure,
  authLoadCurrentUserSuccess,
  authLoginFailure,
  authLoginSubmit,
  authLoginSuccess,
  authLogout,
  authNavigateHomeHandled,
  authResetStatus,
} from './auth.actions';

export const authReducer = createReducer(
  initialAuthState,

  on(authResetStatus, (state) => {
    // Reset message và trạng thái lỗi để UI sạch trước luồng xử lý mới.
    return {
      ...state,
      statusMessage: '',
      isErrorStatus: false,
    };
  }),

  on(authClientValidationFailed, (state, action) => {
    // Cập nhật message lỗi tổng khi validate phía client không đạt.
    return {
      ...state,
      statusMessage: action.message,
      isErrorStatus: true,
      isSubmitting: false,
    };
  }),

  on(authLoginSubmit, (state) => {
    // Bật trạng thái đang submit login để UI disable button / hiển thị spinner cục bộ.
    return {
      ...state,
      isSubmitting: true,
      statusMessage: '',
      isErrorStatus: false,
      shouldNavigateHome: false,
    };
  }),

  on(authLoginSuccess, (state, action) => {
    // Lưu trạng thái xác thực và thông tin user cơ bản sau khi login thành công.
    return {
      ...state,
      isSubmitting: false,
      isAuthenticated: true,
      currentUserName: action.user.userName,
      roles: action.user.roles,
      shouldNavigateHome: true,
      statusMessage: '',
      isErrorStatus: false,
    };
  }),

  on(authLoginFailure, (state, action) => {
    // Tắt submit và hiển thị message lỗi khi login thất bại.
    return {
      ...state,
      isSubmitting: false,
      statusMessage: action.message,
      isErrorStatus: true,
      shouldNavigateHome: false,
    };
  }),

  on(authNavigateHomeHandled, (state) => {
    // Hạ cờ điều hướng về trang chủ để tránh trigger lại khi UI re-render.
    return {
      ...state,
      shouldNavigateHome: false,
    };
  }),

  on(authLogout, (state) => {
    // Reset toàn bộ state về initial để đảm bảo logout sạch.
    return {
      ...initialAuthState,
    };
  }),

  on(authLoadCurrentUser, (state) => {
    // Bật trạng thái loading current user để UI có thể hiển thị trạng thái tải.
    return {
      ...state,
      isLoadingCurrentUser: true,
      currentUser: null,
      currentUserErrorMessage: '',
    };
  }),

  on(authLoadCurrentUserSuccess, (state, action) => {
    // Lưu thông tin user lấy từ /Account/me.
    return {
      ...state,
      isLoadingCurrentUser: false,
      currentUser: action.user,
      currentUserErrorMessage: '',
    };
  }),

  on(authLoadCurrentUserFailure, (state, action) => {
    // Tắt loading và lưu message lỗi để UI hiển thị.
    return {
      ...state,
      isLoadingCurrentUser: false,
      currentUser: null,
      currentUserErrorMessage: action.message,
    };
  })
);
