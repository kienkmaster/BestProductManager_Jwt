import { createFeatureSelector, createSelector } from '@ngrx/store';

import { USER_REGISTER_FEATURE_KEY, UserRegisterState } from '../user-register/user-register.store';

/**
 * Feature selector cho state đăng ký user.
 */
export const selectUserRegisterState =
  createFeatureSelector<UserRegisterState>(USER_REGISTER_FEATURE_KEY);

/**
 * Selector lấy cờ submitting (spinner).
 */
export const selectUserRegisterIsSubmitting = createSelector(
  selectUserRegisterState,
  (state: UserRegisterState) => state.isSubmitting
);

/**
 * Selector lấy status message.
 */
export const selectUserRegisterStatusMessage = createSelector(
  selectUserRegisterState,
  (state: UserRegisterState) => state.statusMessage
);

/**
 * Selector lấy cờ lỗi của status.
 */
export const selectUserRegisterIsErrorStatus = createSelector(
  selectUserRegisterState,
  (state: UserRegisterState) => state.isErrorStatus
);

/**
 * Selector lấy cờ yêu cầu reset form sau success.
 */
export const selectUserRegisterShouldResetForm = createSelector(
  selectUserRegisterState,
  (state: UserRegisterState) => state.shouldResetForm
);

/**
 * Selector lấy userName vừa đăng ký thành công.
 */
export const selectUserRegisterLastRegisteredUserName = createSelector(
  selectUserRegisterState,
  (state: UserRegisterState) => state.lastRegisteredUserName
);
