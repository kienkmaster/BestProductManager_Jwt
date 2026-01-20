import { createFeatureSelector, createSelector } from '@ngrx/store';

import { LOADING_FEATURE_KEY, LoadingState } from './loading.store';

/**
 * Feature selector cho LoadingState.
 */
export const selectLoadingState = createFeatureSelector<LoadingState>(LOADING_FEATURE_KEY);

/**
 * Selector trả về true khi có ít nhất 1 request đang hoạt động.
 */
export const selectIsLoading = createSelector(
  selectLoadingState,
  (state: LoadingState) => state.activeRequests > 0
);
