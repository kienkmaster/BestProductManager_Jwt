import { Component, EventEmitter, Output } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AuthUserModel } from '../../models/auth-user.model';
import {
  selectAuthCurrentUser,
  selectAuthCurrentUserErrorMessage,
  selectAuthIsLoadingCurrentUser,
} from '../../stores/auth/auth.selector';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss'],
  standalone: false
})
export class UserInfoComponent {
  /**
   * Sự kiện yêu cầu quay về trang chủ.
   */
  @Output()
  public readonly backToHome = new EventEmitter<void>();

  /**
   * Observable: trạng thái loading thông tin user hiện tại.
   */
  public readonly isLoading$: Observable<boolean>;

  /**
   * Observable: dữ liệu user hiện tại.
   */
  public readonly currentUser$: Observable<AuthUserModel | null>;

  /**
   * Observable: message lỗi khi tải user info thất bại.
   */
  public readonly errorMessage$: Observable<string>;

  /**
   * Khởi tạo UserInfoComponent.
   * @param store NgRx Store dùng để select state.
   */
  constructor(private readonly store: Store) {
    // Select trạng thái loading.
    this.isLoading$ = this.store.select(selectAuthIsLoadingCurrentUser);

    // Select dữ liệu current user.
    this.currentUser$ = this.store.select(selectAuthCurrentUser);

    // Select message lỗi.
    this.errorMessage$ = this.store.select(selectAuthCurrentUserErrorMessage);
  }
}
