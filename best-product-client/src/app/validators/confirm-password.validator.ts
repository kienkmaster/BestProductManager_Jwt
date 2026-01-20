import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validator kiểm tra xác nhận mật khẩu (confirm password validator) trên FormGroup.
 * @param passwordControlName Tên control chứa password.
 * @param confirmPasswordControlName Tên control chứa confirmPassword.
 * @returns ValidatorFn gắn vào FormGroup để kiểm tra 2 field khớp nhau.
 */
export function confirmPasswordValidator(
  passwordControlName: string,
  confirmPasswordControlName: string
): ValidatorFn {
  /**
   * Trả về hàm validator áp dụng trên AbstractControl cấp group.
   * @param control AbstractControl cấp group.
   * @returns ValidationErrors hoặc null nếu hợp lệ.
   */
  return (control: AbstractControl): ValidationErrors | null => {
    /**
     * Lấy password control và confirmPassword control theo tên cấu hình.
     */
    const passwordControl = control.get(passwordControlName);
    const confirmPasswordControl = control.get(confirmPasswordControlName);

    /**
     * Nếu không tìm thấy control: không đánh lỗi, tránh gây crash cấu hình form.
     */
    if (!passwordControl || !confirmPasswordControl) {
      return null;
    }

    /**
     * Lấy giá trị hiện tại để so sánh.
     */
    const password: unknown = passwordControl.value;
    const confirmPassword: unknown = confirmPasswordControl.value;

    /**
     * Nếu khớp: hợp lệ.
     */
    if (password === confirmPassword) {
      return null;
    }

    /**
     * Không khớp: trả lỗi confirmPasswordMismatch để UI hiển thị.
     */
    return {
      confirmPasswordMismatch: true,
    };
  };
}
