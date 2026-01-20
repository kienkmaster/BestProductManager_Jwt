import { AuthUserModel } from '../../models/auth-user.model';

/**
 * Feature key cho Auth store.
 */
export const AUTH_FEATURE_KEY = 'auth';

/**
 * AuthState (trạng thái xác thực) quản lý login + current user info.
 */
export type AuthState = {
  isSubmitting: boolean;
  statusMessage: string;
  isErrorStatus: boolean;

  isAuthenticated: boolean;
  currentUserName: string;
  roles: string[];

  shouldNavigateHome: boolean;

  isLoadingCurrentUser: boolean;
  currentUser: AuthUserModel | null;
  currentUserErrorMessage: string;
};

/**
 * Trạng thái khởi tạo (initial state) cho AuthState.
 */
export const initialAuthState: AuthState = {
  isSubmitting: false,
  statusMessage: '',
  isErrorStatus: false,

  isAuthenticated: false,
  currentUserName: '',
  roles: [],

  shouldNavigateHome: false,

  isLoadingCurrentUser: false,
  currentUser: null,
  currentUserErrorMessage: '',
};
