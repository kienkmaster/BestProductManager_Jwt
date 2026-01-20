import { createReducer, on } from '@ngrx/store';

import { initialLoadingState } from './loading.store';

import { loadingRequestEnded, loadingRequestStarted } from './loading.actions';

export const loadingReducer = createReducer(
  initialLoadingState,

  on(loadingRequestStarted, (state) => {
    // Tăng số lượng request đang hoạt động để bật trạng thái loading.
    return {
      ...state,
      activeRequests: state.activeRequests + 1,
    };
  }),

  on(loadingRequestEnded, (state) => {
    // Giảm số lượng request đang hoạt động và đảm bảo không nhỏ hơn 0.
    const nextCount = state.activeRequests - 1;

    return {
      ...state,
      activeRequests: nextCount < 0 ? 0 : nextCount,
    };
  })
);
