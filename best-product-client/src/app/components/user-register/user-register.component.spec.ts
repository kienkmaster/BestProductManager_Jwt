import { Component, DestroyRef, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, filter, withLatestFrom, tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RegisterUserModel } from '../../models/register-user.model';
import { passwordStrengthValidator } from '../../validators/password-strength.validator';
import { confirmPasswordValidator } from '../../validators/confirm-password.validator';

import {
  userRegisterClientValidationFailed,
  userRegisterResetFormHandled,
  userRegisterResetStatus,
  userRegisterSubmit,
} from '../../stores/user-register/user-register.actions';

import {
  selectUserRegisterIsErrorStatus,
  selectUserRegisterIsSubmitting,
  selectUserRegisterLastRegisteredUserName,
  selectUserRegisterShouldResetForm,
  selectUserRegisterStatusMessage,
} from '../../stores/user-register/user-register.selector';

type UserRegisterFormControls = {
  userName: FormControl<string>;
  password: FormControl<string>;
  confirmPassword: FormControl<string>;
};

@Component({
  selector: 'app-user-register',
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.scss'],
})
export class UserRegisterComponent {
  /**
   * FormGroup (nhóm form) theo typed reactive forms (form phản ứng typed).
   */
  public readonly form: FormGroup<UserRegisterFormControls>;

  /**
   * Observable cờ submitting (submitting flag) lấy từ NgRx Store để điều khiển spinner.
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
   * Sự kiện báo đăng ký thành công (register success) để mở rộng xử lý ở component cha nếu cần.
   */
  @Output()
  public readonly registerSuccess: EventEmitter<string> = new EventEmitter<string>();

  /**
   * Khởi tạo component đăng ký (UserRegisterComponent).
   * @param frmBuilder FormBuilder dùng tạo typed reactive form.
   * @param store NgRx Store dùng để dispatch action và select state.
   * @param destroyRef DestroyRef dùng để tự động hủy subscription theo vòng đời component.
   */
  constructor(
  private readonly frmBuilder: FormBuilder,
  private readonly store: Store,
  private readonly destroyRef: DestroyRef
  ) {
  /**
   * Khởi tạo form ngay khi component được tạo để template có thể bind.
   */
  this.form = this.buildForm();

  /**
   * Khởi tạo các observable selector sau khi Store đã được inject.
   * Mục tiêu: tránh lỗi TS2729 (used before initialization) do property initializer tham chiếu store.
   */
  this.isSubmitting$ = this.store.select(selectUserRegisterIsSubmitting);
  this.statusMessage$ = this.store.select(selectUserRegisterStatusMessage);
  this.isErrorStatus$ = this.store.select(selectUserRegisterIsErrorStatus);

  /**
   * Thiết lập luồng xử lý reset form sau khi đăng ký thành công theo state của store.
   */
  this.initializeResetFormListener();
  }

  /**
   * Tạo form đăng ký (build register form).
   * Bao gồm:
   * - Built-in validators (required/minLength/maxLength)
   * - Custom validators (passwordStrength, confirmPassword)
   * @returns FormGroup typed cho đăng ký user.
   */
  private buildForm(): FormGroup<UserRegisterFormControls> {
  /**
   * Tạo group với nonNullable để tránh null trong typed forms.
   */
  const group = this.frmBuilder.nonNullable.group(
    {
    /**
     * userName: bắt buộc, giới hạn độ dài cho tính ổn định dữ liệu.
     */
    userName: this.frmBuilder.nonNullable.control(
      '',
      {
      validators: [
        Validators.required,
        Validators.maxLength(50),
      ],
      }
    ),

    /**
     * password: bắt buộc, giới hạn min/max, và kiểm tra độ mạnh bằng custom validator.
     */
    password: this.frmBuilder.nonNullable.control(
      '',
      {
      validators: [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(20),
        passwordStrengthValidator(),
      ],
      }
    ),

    /**
     * confirmPassword: bắt buộc, giới hạn độ dài.
     * Kiểm tra khớp với password được thực hiện ở validator cấp group.
     */
    confirmPassword: this.frmBuilder.nonNullable.control(
      '',
      {
      validators: [
        Validators.required,
        Validators.maxLength(20),
      ],
      }
    ),
    },
    {
    /**
     * Validator cấp group để đảm bảo password và confirmPassword khớp nhau.
     */
    validators: [
      confirmPasswordValidator('password', 'confirmPassword'),
    ],
    }
  );

  /**
   * Trả về form group đã cấu hình đầy đủ validators.
   */
  return group;
  }

  /**
   * Xử lý submit (submit) form đăng ký.
   * - Reset status trong store
   * - Validate form
   * - Map form sang model (DTO-like)
   * - Dispatch action submit để effect gọi API và reducer quản lý state (spinner/status)
   * @returns void
   */
  public onSubmit(): void {
  /**
   * Reset status cũ trong store trước khi xử lý submit mới.
   */
  this.store.dispatch(userRegisterResetStatus());

  /**
   * Nếu form không hợp lệ: đánh dấu touched để hiển thị lỗi theo field.
   */
  if (this.form.invalid) {
    /**
     * Đánh dấu tất cả control là touched để bật hiển thị message lỗi theo field.
     */
    this.form.markAllAsTouched();

    /**
     * Dispatch lỗi validate phía client để status hiển thị dưới form.
     */
    this.store.dispatch(userRegisterClientValidationFailed({
    message: 'Thông tin đăng ký chưa hợp lệ. Vui lòng kiểm tra lại.',
    }));

    return;
  }

  /**
   * Map form value sang model để gửi API theo chuẩn business (DTO-like).
   */
  const model = this.mapFormToModel();

  /**
   * Dispatch action submit để effect gọi API đăng ký.
   */
  this.store.dispatch(userRegisterSubmit({ model }));
  }

  /**
   * Map dữ liệu form sang RegisterUserModel.
   * @returns RegisterUserModel dùng để gửi API.
   */
  private mapFormToModel(): RegisterUserModel {
  /**
   * Lấy dữ liệu raw (getRawValue) để đảm bảo lấy đủ control value.
   */
  const raw = this.form.getRawValue();

  /**
   * Tạo model theo cấu trúc request mà API mong đợi.
   */
  return new RegisterUserModel(
    raw.userName,
    raw.password,
    raw.confirmPassword
  );
  }

  /**
   * Thiết lập listener để reset form sau khi store báo đăng ký thành công.
   * @returns void
   */
  private initializeResetFormListener(): void {
  /**
   * Select cờ shouldResetForm và xử lý reset form khi cờ được bật.
   */
  this.store.select(selectUserRegisterShouldResetForm)
    .pipe(
    /**
     * Chỉ xử lý khi store yêu cầu reset form.
     */
    filter((shouldResetForm: boolean) => shouldResetForm === true),

    /**
     * Lấy thêm userName vừa đăng ký thành công để phục vụ event mở rộng nếu cần.
     */
    withLatestFrom(this.store.select(selectUserRegisterLastRegisteredUserName)),

    tap(([_, userName]) => {
      /**
       * Clear dữ liệu đã nhập để không giữ thông tin nhạy cảm (password) trên UI.
       */
      this.clearFormInputsAfterSuccess();

      /**
       * Hạ cờ reset trong store để tránh reset lặp.
       * ※ Dispatch: gửi một action vào store.
       */
      this.store.dispatch(userRegisterResetFormHandled());

      /**
       * Phát event đăng ký thành công để mở rộng xử lý ở component cha nếu cần.
       */
      if (typeof userName === 'string' && userName.trim().length > 0) {
      this.registerSuccess.emit(userName);
      }
    }),

    /**
     * Tự động hủy subscription theo vòng đời component.
     */
    takeUntilDestroyed(this.destroyRef)
    )
    .subscribe();
  }

  /**
   * Clear dữ liệu đã nhập sau khi đăng ký thành công.
   * @returns void
   */
  private clearFormInputsAfterSuccess(): void {
  /**
   * Reset giá trị về rỗng để không giữ thông tin nhạy cảm (password) trên UI.
   */
  this.form.reset({
    userName: '',
    password: '',
    confirmPassword: '',
  });

  /**
   * Làm sạch trạng thái tương tác để không hiển thị lỗi ngay sau khi reset.
   */
  this.form.markAsPristine();

  /**
   * Làm sạch trạng thái touched để không kích hoạt hiển thị message theo field.
   */
  this.form.markAsUntouched();

  /**
   * Cập nhật lại trạng thái validate sau khi reset.
   */
  this.form.updateValueAndValidity();
  }

  /**
   * Lấy thông điệp lỗi cho userName dựa trên trạng thái validator.
   * @returns Chuỗi lỗi hoặc rỗng.
   */
  public getUserNameErrorMessage(): string {
  /**
   * Lấy control userName để kiểm tra lỗi.
   */
  const control = this.form.controls.userName;

  /**
   * Chỉ hiển thị lỗi khi user đã tương tác (touched).
   */
  if (!control.touched) {
    return '';
  }

  /**
   * Lỗi required.
   */
  if (control.hasError('required')) {
    return 'Vui lòng nhập tên người dùng.';
  }

  /**
   * Lỗi maxLength.
   */
  if (control.hasError('maxlength')) {
    return 'Tên người dùng không được vượt quá 50 ký tự.';
  }

  /**
   * Không có lỗi phù hợp.
   */
  return '';
  }

  /**
   * Lấy thông điệp lỗi cho password dựa trên trạng thái validator.
   * @returns Chuỗi lỗi hoặc rỗng.
   */
  public getPasswordErrorMessage(): string {
  /**
   * Lấy control password để kiểm tra lỗi.
   */
  const control = this.form.controls.password;

  /**
   * Chỉ hiển thị lỗi khi user đã tương tác (touched).
   */
  if (!control.touched) {
    return '';
  }

  /**
   * Lỗi required.
   */
  if (control.hasError('required')) {
    return 'Vui lòng nhập mật khẩu.';
  }

  /**
   * Lỗi minLength.
   */
  if (control.hasError('minlength')) {
    return 'Mật khẩu phải có tối thiểu 6 ký tự.';
  }

  /**
   * Lỗi maxLength.
   */
  if (control.hasError('maxlength')) {
    return 'Mật khẩu không được vượt quá 20 ký tự.';
  }

  /**
   * Lỗi custom passwordStrength.
   */
  if (control.hasError('passwordStrength')) {
    return 'Mật khẩu phải có chữ hoa, chữ thường và chữ số.';
  }

  /**
   * Không có lỗi phù hợp.
   */
  return '';
  }

  /**
   * Lấy thông điệp lỗi cho confirmPassword dựa trên validator field và validator group.
   * @returns Chuỗi lỗi hoặc rỗng.
   */
  public getConfirmPasswordErrorMessage(): string {
  /**
   * Lấy control confirmPassword để kiểm tra lỗi.
   */
  const control = this.form.controls.confirmPassword;

  /**
   * Chỉ hiển thị lỗi khi user đã tương tác (touched).
   */
  if (!control.touched) {
    return '';
  }

  /**
   * Lỗi required.
   */
  if (control.hasError('required')) {
    return 'Vui lòng xác nhận mật khẩu.';
  }

  /**
   * Lỗi group confirmPasswordMismatch.
   */
  if (this.form.hasError('confirmPasswordMismatch')) {
    return 'Xác nhận mật khẩu không khớp.';
  }

  /**
   * Không có lỗi phù hợp.
   */
  return '';
  }
}
