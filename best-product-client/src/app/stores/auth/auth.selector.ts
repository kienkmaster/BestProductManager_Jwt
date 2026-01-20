import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AUTH_FEATURE_KEY, AuthState } from './auth.store';

/**
 * Feature selector cho AuthState.
 */
export const selectAuthState = createFeatureSelector<AuthState>(AUTH_FEATURE_KEY);

/**
 * Selector: trạng thái đang submit login.
 */
export const selectAuthIsSubmitting = createSelector(
  selectAuthState,
  (state: AuthState) => state.isSubmitting
);

/**
 * Selector: message status của login.
 */
export const selectAuthStatusMessage = createSelector(
  selectAuthState,
  (state: AuthState) => state.statusMessage
);

/**
 * Selector: cờ lỗi status của login.
 */
export const selectAuthIsErrorStatus = createSelector(
  selectAuthState,
  (state: AuthState) => state.isErrorStatus
);

/**
 * Selector: trạng thái đã xác thực.
 */
export const selectAuthIsAuthenticated = createSelector(
  selectAuthState,
  (state: AuthState) => state.isAuthenticated
);

/**
 * Selector: userName đang đăng nhập.
 */
export const selectAuthCurrentUserName = createSelector(
  selectAuthState,
  (state: AuthState) => state.currentUserName
);

/**
 * Selector: cờ điều hướng về trang chủ sau login.
 */
export const selectAuthShouldNavigateHome = createSelector(
  selectAuthState,
  (state: AuthState) => state.shouldNavigateHome
);

/**
 * Selector: trạng thái loading thông tin current user.
 */
export const selectAuthIsLoadingCurrentUser = createSelector(
  selectAuthState,
  (state: AuthState) => state.isLoadingCurrentUser
);

/**
 * Selector: dữ liệu current user lấy từ /Account/me.
 */
export const selectAuthCurrentUser = createSelector(
  selectAuthState,
  (state: AuthState) => state.currentUser
);

/**
 * Selector: message lỗi khi tải current user thất bại.
 */
export const selectAuthCurrentUserErrorMessage = createSelector(
  selectAuthState,
  (state: AuthState) => state.currentUserErrorMessage
);
