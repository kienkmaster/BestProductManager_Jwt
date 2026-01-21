import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

/**
 * LoadingState (trạng thái loading) quản lý số lượng request API đang hoạt động.
 */
export interface LoadingState {
  activeRequests: number;
}

/**
 * Initial state (trạng thái khởi tạo) cho LoadingState.
 */
const initialLoadingState: LoadingState = {
  activeRequests: 0,
};

@Injectable({
  providedIn: 'root'
})
export class LoadingStateService {
  /**
   * BehaviorSubject giữ trạng thái loading hiện tại.
   */
  private readonly stateSubject = new BehaviorSubject<LoadingState>(initialLoadingState);

  /**
   * Observable để component subscribe và nhận state updates.
   */
  public readonly state$: Observable<LoadingState> = this.stateSubject.asObservable();

  /**
   * Getter để truy cập state hiện tại.
   */
  public get currentState(): LoadingState {
    return this.stateSubject.getValue();
  }

  /**
   * Selector trả về true khi có ít nhất 1 request đang hoạt động.
   */
  public get isLoading$(): Observable<boolean> {
    return this.state$.pipe(
      map(state => state.activeRequests > 0)
    );
  }

  /**
   * Bắt đầu một request API để bật trạng thái loading.
   */
  public requestStarted(): void {
    this.stateSubject.next({
      ...this.currentState,
      activeRequests: this.currentState.activeRequests + 1,
    });
  }

  /**
   * Kết thúc một request API để tắt trạng thái loading khi không còn request nào.
   */
  public requestEnded(): void {
    const nextCount = this.currentState.activeRequests - 1;
    this.stateSubject.next({
      ...this.currentState,
      activeRequests: nextCount < 0 ? 0 : nextCount,
    });
  }
}
