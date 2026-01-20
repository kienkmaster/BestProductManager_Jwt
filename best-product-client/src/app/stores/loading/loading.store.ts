/**
 * Feature key cho Loading store.
 */
export const LOADING_FEATURE_KEY = 'loading';

/**
 * LoadingState (trạng thái loading) quản lý số lượng request API đang hoạt động.
 */
export type LoadingState = {
  activeRequests: number;
};

/**
 * Initial state (trạng thái khởi tạo) cho LoadingState.
 */
export const initialLoadingState: LoadingState = {
  activeRequests: 0,
};
