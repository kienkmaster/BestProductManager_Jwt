import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthUserModel } from '../../models/auth-user.model';

/**
 * AuthState (trạng thái xác thực) quản lý login + current user info.
 */
export interface AuthState {
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
}

/**
 * Trạng thái khởi tạo (initial state) cho AuthState.
 */
const initialAuthState: AuthState = {
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

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  /**
   * BehaviorSubject giữ trạng thái xác thực hiện tại.
   */
  private readonly stateSubject = new BehaviorSubject<AuthState>(initialAuthState);

  /**
   * Observable để component subscribe và nhận state updates.
   */
  public readonly state$: Observable<AuthState> = this.stateSubject.asObservable();

  /**
   * Getter để truy cập state hiện tại.
   */
  public get currentState(): AuthState {
    return this.stateSubject.getValue();
  }

  /**
   * Selector: trạng thái đang submit login.
   */
  public get isSubmitting$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isSubmitting));
    });
  }

  /**
   * Selector: message status của login.
   */
  public get statusMessage$(): Observable<string> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.statusMessage));
    });
  }

  /**
   * Selector: cờ lỗi status của login.
   */
  public get isErrorStatus$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isErrorStatus));
    });
  }

  /**
   * Selector: trạng thái đã xác thực.
   */
  public get isAuthenticated$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isAuthenticated));
    });
  }

  /**
   * Selector: userName đang đăng nhập.
   */
  public get currentUserName$(): Observable<string> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.currentUserName));
    });
  }

  /**
   * Selector: cờ điều hướng về trang chủ sau login.
   */
  public get shouldNavigateHome$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.shouldNavigateHome));
    });
  }

  /**
   * Selector: trạng thái loading thông tin current user.
   */
  public get isLoadingCurrentUser$(): Observable<boolean> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.isLoadingCurrentUser));
    });
  }

  /**
   * Selector: dữ liệu current user lấy từ /Account/me.
   */
  public get currentUser$(): Observable<AuthUserModel | null> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.currentUser));
    });
  }

  /**
   * Selector: message lỗi khi tải current user thất bại.
   */
  public get currentUserErrorMessage$(): Observable<string> {
    return new Observable(subscriber => {
      this.state$.subscribe(state => subscriber.next(state.currentUserErrorMessage));
    });
  }

  /**
   * Reset status (message/error) dùng chung cho feature Auth.
   */
  public resetStatus(): void {
    this.stateSubject.next({
      ...this.currentState,
      statusMessage: '',
      isErrorStatus: false,
    });
  }

  /**
   * Ghi nhận lỗi validate phía client (client validation) trong luồng login.
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
   * Submit login (bắt đầu đăng nhập).
   */
  public loginSubmit(): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: true,
      statusMessage: '',
      isErrorStatus: false,
      shouldNavigateHome: false,
    });
  }

  /**
   * Login thành công (payload user).
   */
  public loginSuccess(user: AuthUserModel): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: false,
      isAuthenticated: true,
      currentUserName: user.userName,
      roles: user.roles,
      shouldNavigateHome: true,
      statusMessage: '',
      isErrorStatus: false,
    });
  }

  /**
   * Login thất bại (payload message).
   */
  public loginFailure(message: string): void {
    this.stateSubject.next({
      ...this.currentState,
      isSubmitting: false,
      statusMessage: message,
      isErrorStatus: true,
      shouldNavigateHome: false,
    });
  }

  /**
   * Đánh dấu đã xử lý điều hướng về trang chủ sau login.
   */
  public navigateHomeHandled(): void {
    this.stateSubject.next({
      ...this.currentState,
      shouldNavigateHome: false,
    });
  }

  /**
   * Logout.
   */
  public logout(): void {
    this.stateSubject.next({
      ...initialAuthState,
    });
  }

  /**
   * User click link tên user để yêu cầu tải thông tin user hiện tại (GET /Account/me).
   */
  public loadCurrentUser(): void {
    this.stateSubject.next({
      ...this.currentState,
      isLoadingCurrentUser: true,
      currentUser: null,
      currentUserErrorMessage: '',
    });
  }

  /**
   * Tải thông tin user hiện tại thành công.
   */
  public loadCurrentUserSuccess(user: AuthUserModel): void {
    this.stateSubject.next({
      ...this.currentState,
      isLoadingCurrentUser: false,
      currentUser: user,
      currentUserErrorMessage: '',
    });
  }

  /**
   * Tải thông tin user hiện tại thất bại.
   */
  public loadCurrentUserFailure(message: string): void {
    this.stateSubject.next({
      ...this.currentState,
      isLoadingCurrentUser: false,
      currentUser: null,
      currentUserErrorMessage: message,
    });
  }
}
