import { createAction } from '@ngrx/store';

/**
 * Bắt đầu một request API để bật trạng thái loading.
 */
export const loadingRequestStarted = createAction('[Loading] Request Started');

/**
 * Kết thúc một request API để tắt trạng thái loading khi không còn request nào.
 */
export const loadingRequestEnded = createAction('[Loading] Request Ended');
