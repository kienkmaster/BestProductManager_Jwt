import { Component, DestroyRef, EventEmitter, Output } from '@angular/core';

import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

import { Store } from '@ngrx/store';

import { Observable, filter, tap, withLatestFrom } from 'rxjs';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { LoginRequestModel } from '../../models/login-request.model';

import { buildLoginPasswordValidators, buildLoginUserNameValidators } from '../../validators/login.validator';

import {
  authClientValidationFailed,
  authLoginSubmit,
  authNavigateHomeHandled,
  authResetStatus,
} from '../../stores/auth/auth.actions';

import {
  selectAuthCurrentUserName,
  selectAuthIsErrorStatus,
  selectAuthIsSubmitting,
  selectAuthShouldNavigateHome,
  selectAuthStatusMessage,
} from '../../stores/auth/auth.selector';

type UserAuthFormControls = {
  userName: FormControl<string>;
  password: FormControl<string>;
};

@Component({
  selector: 'app-user-auth',
  templateUrl: './user-auth.component.html',
  styleUrls: ['./user-auth.component.scss'],
  standalone: false
})
export class UserAuthComponent {
  /**
   * FormGroup (nhóm form) theo typed reactive forms (form phản ứng typed) cho đăng nhập.
   */
  public readonly form: FormGroup<UserAuthFormControls>;

  /**
   * Observable cờ submitting lấy từ NgRx Store để điều khiển spinner.
   */
  public readonly isSubmitting$: Observable<boolean>;

  /**
   * Observable status message lấy từ NgRx Store để hiển thị dưới form.
   */
  public readonly statusMessage$: Observable<string>;

  /**
   * Observable cờ lỗi của status lấy từ NgRx Store để điều khiển style status.
   */
  public readonly isErrorStatus$: Observable<boolean>;

  /**
   * Sự kiện báo đăng nhập thành công để component cha điều phối về trang chủ.
   */
  @Output()
  public readonly loginSuccess: EventEmitter<string> = new EventEmitter<string>();

  /**
   * Khởi tạo UserAuthComponent.
   * @param frmBuilder FormBuilder dùng tạo typed reactive form.
   * @param store NgRx Store dùng để dispatch action và select state.
   * @param destroyRef DestroyRef dùng để tự động hủy subscription theo vòng đời component.
   */
  constructor(
    private readonly frmBuilder: FormBuilder,
    private readonly store: Store,
    private readonly destroyRef: DestroyRef
  ) {
    // Khởi tạo form ngay khi component được tạo để template có thể bind.
    this.form = this.buildForm();

    // Khởi tạo các observable selector sau khi Store đã được inject.
    this.isSubmitting$ = this.store.select(selectAuthIsSubmitting);
    this.statusMessage$ = this.store.select(selectAuthStatusMessage);
    this.isErrorStatus$ = this.store.select(selectAuthIsErrorStatus);

    // Thiết lập listener điều hướng về trang chủ sau khi login thành công theo state của store.
    this.initializeNavigateHomeListener();
  }

  /**
   * Tạo form đăng nhập.
   * @returns FormGroup typed cho đăng nhập.
   */
  private buildForm(): FormGroup<UserAuthFormControls> {
    // Tạo group với nonNullable để tránh null trong typed forms.
    const group = this.frmBuilder.nonNullable.group({
      userName: this.frmBuilder.nonNullable.control('', {
        validators: buildLoginUserNameValidators(),
      }),
      password: this.frmBuilder.nonNullable.control('', {
        validators: buildLoginPasswordValidators(),
      }),
    });

    return group;
  }

  /**
   * Xử lý submit form đăng nhập.
   * @returns void
   */
  public onSubmit(): void {
    // Reset status cũ trong store trước khi xử lý submit mới.
    this.store.dispatch(authResetStatus());

    // Nếu form không hợp lệ: đánh dấu touched để hiển thị lỗi theo field.
    if (this.form.invalid) {
      // Đánh dấu tất cả control là touched để bật hiển thị message lỗi theo field.
      this.form.markAllAsTouched();

      // Dispatch lỗi validate phía client để status hiển thị dưới form.
      this.store.dispatch(authClientValidationFailed({
        message: 'Thông tin đăng nhập chưa hợp lệ. Vui lòng kiểm tra lại.',
      }));

      return;
    }

    // Map form value sang model để gửi API theo chuẩn business.
    const model = this.mapFormToModel();

    // Dispatch action submit để effect gọi API đăng nhập.
    this.store.dispatch(authLoginSubmit({ model }));
  }

  /**
   * Map dữ liệu form sang LoginRequestModel.
   * @returns LoginRequestModel dùng để gửi API.
   */
  private mapFormToModel(): LoginRequestModel {
    // Lấy dữ liệu raw (getRawValue) để đảm bảo lấy đủ control value.
    const raw = this.form.getRawValue();

    // Tạo model theo cấu trúc request mà API mong đợi.
    return new LoginRequestModel(raw.userName, raw.password);
  }

  /**
   * Lắng nghe cờ điều hướng về trang chủ sau login success.
   * @returns void
   */
  private initializeNavigateHomeListener(): void {
    // Select cờ shouldNavigateHome và xử lý khi cờ được bật.
    this.store.select(selectAuthShouldNavigateHome)
      .pipe(
        // Chỉ xử lý khi store yêu cầu điều hướng.
        filter((shouldNavigateHome: boolean) => shouldNavigateHome === true),

        // Lấy thêm userName hiện tại để emit event cho component cha.
        withLatestFrom(this.store.select(selectAuthCurrentUserName)),

        tap(([_, userName]) => {
          // Hạ cờ điều hướng trong store để tránh trigger lặp.
          this.store.dispatch(authNavigateHomeHandled());

          // Emit event để AppComponent điều phối về trang chủ và cập nhật menu.
          if (typeof userName === 'string' && userName.trim().length > 0) {
            this.loginSuccess.emit(userName);
          }
        }),

        // Tự động hủy subscription theo vòng đời component.
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  /**
   * Lấy thông điệp lỗi cho userName dựa trên trạng thái validator.
   * @returns Chuỗi lỗi hoặc rỗng.
   */
  public getUserNameErrorMessage(): string {
    // Lấy control userName để kiểm tra lỗi.
    const control = this.form.controls.userName;

    // Chỉ hiển thị lỗi khi user đã tương tác (touched).
    if (!control.touched) {
      return '';
    }

    // Lỗi required.
    if (control.hasError('required')) {
      return 'Vui lòng nhập tên đăng nhập.';
    }

    // Lỗi maxLength.
    if (control.hasError('maxlength')) {
      return 'Tên đăng nhập không được vượt quá 50 ký tự.';
    }

    // Lỗi custom whitespaceNotAllowed.
    if (control.hasError('whitespaceNotAllowed')) {
      return 'Tên đăng nhập không được chứa khoảng trắng.';
    }

    return '';
  }

  /**
   * Lấy thông điệp lỗi cho password dựa trên trạng thái validator.
   * @returns Chuỗi lỗi hoặc rỗng.
   */
  public getPasswordErrorMessage(): string {
    // Lấy control password để kiểm tra lỗi.
    const control = this.form.controls.password;

    // Chỉ hiển thị lỗi khi user đã tương tác (touched).
    if (!control.touched) {
      return '';
    }

    // Lỗi required.
    if (control.hasError('required')) {
      return 'Vui lòng nhập mật khẩu.';
    }

    // Lỗi minLength.
    if (control.hasError('minlength')) {
      return 'Mật khẩu phải có tối thiểu 6 ký tự.';
    }

    // Lỗi maxLength.
    if (control.hasError('maxlength')) {
      return 'Mật khẩu không được vượt quá 20 ký tự.';
    }

    // Lỗi custom whitespaceNotAllowed.
    if (control.hasError('whitespaceNotAllowed')) {
      return 'Mật khẩu không được chứa khoảng trắng.';
    }

    return '';
  }
}
