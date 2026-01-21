import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

/**
 * Trạng thái đăng ký user (UserRegisterState) quản lý bằng Service.
 */
export interface UserRegisterState {
  /**
   * Cờ đang submit (submitting) để điều khiển spinner và khóa nút submit.
   */
  isSubmitting: boolean;

  /**
   * Thông điệp trạng thái (status message) hiển thị dưới form.
   */
  statusMessage: string;

  /**
   * Cờ xác định status là lỗi (error flag).
   */
  isErrorStatus: boolean;

  /**
   * Cờ yêu cầu reset form sau khi đăng ký thành công.
   * Component sẽ xử lý reset form và gọi method "resetFormHandled".
   */
  shouldResetForm: boolean;

  /**
   * Tên user vừa đăng ký thành công, phục vụ các xử lý mở rộng (optional).
   */
  lastRegisteredUserName: string | null;
}

/**
 * Initial state (trạng thái ban đầu) của đăng ký user.
 */
const initialUserRegisterState: UserRegisterState = {
  isSubmitting: false,
  statusMessage: '',
  isErrorStatus: false,
  shouldResetForm: false,
  lastRegisteredUserName: null,
};

@Injectable({
  providedIn: 'root'
})
export class UserRegisterStateService {
  /**
   * BehaviorSubject giữ trạng thái đăng ký hiện tại.
   */
  private readonly stateSubject = new BehaviorSubject<UserRegisterState>(initialUserRegisterState);

  /**
   * Observable để component subscribe và nhận state updates.
   */
  public readonly state$: Observable<UserRegisterState> = this.stateSubject.asObservable();

  /**
   * Getter để truy cập state hiện tại.
   */
  public get currentState(): UserRegisterState {
    return this.stateSubject.getValue();
  }

  /**
   * Selector: cờ submitting (spinner).
   */
  public get isSubmitting$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isSubmitting));
    });
  }

  /**
   * Selector: status message.
   */
  public get statusMessage$(): Observable<string> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.statusMessage));
    });
  }

  /**
   * Selector: cờ lỗi của status.
   */
  public get isErrorStatus$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isErrorStatus));
    });
  }

  /**
   * Selector: cờ yêu cầu reset form sau success.
   */
  public get shouldResetForm$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.shouldResetForm));
    });
  }

  /**
   * Selector: userName vừa đăng ký thành công.
   */
  public get lastRegisteredUserName$(): Observable<string | null> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.lastRegisteredUserName));
    });
  }

  /**
   * Reset trạng thái status (status state) của đăng ký user.
   * Mục tiêu: xóa status cũ trước khi thực hiện submit mới.
   */
  public resetStatus(): void {
    this.stateSubject.next({
      ...this.currentState,
      statusMessage: '',
      isErrorStatus: false,
    });
  }

  /**
   * Báo lỗi validate phía client (client-side validation) khi submit form không hợp lệ.
   * @param message Thông điệp lỗi cần hiển thị.
   */
  public clientValidationFailed(message: string): void {
    this.stateSubject.next({
      ...this.currentState,
      statusMessage: message,
      isErrorStatus: true,
      isSubmitting: false,
    });
  }

  /**
   * Submit đăng ký user (submit register).
   */
  public submit(): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: true,
      statusMessage: '',
      isErrorStatus: false,
      shouldResetForm: false,
    });
  }

  /**
   * Đăng ký thành công (register success).
   * @param message Thông điệp thành công từ backend hoặc message mặc định.
   * @param userName Tên user vừa đăng ký thành công.
   */
  public success(message: string, userName: string): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: false,
      statusMessage: message,
      isErrorStatus: false,
      shouldResetForm: true,
      lastRegisteredUserName: userName,
    });
  }

  /**
   * Đăng ký thất bại (register failure).
   * @param message Thông điệp lỗi từ backend hoặc message mặc định.
   */
  public failure(message: string): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: false,
      statusMessage: message,
      isErrorStatus: true,
      shouldResetForm: false,
    });
  }

  /**
   * Xác nhận đã xử lý reset form (reset handled) sau khi store yêu cầu reset.
   * Mục tiêu: tránh lặp reset form khi state vẫn đang giữ cờ reset.
   */
  public resetFormHandled(): void {
    this.stateSubject.next({
      ...this.currentState,
      shouldResetForm: false,
    });
  }
}
