import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validator kiểm tra độ mạnh mật khẩu (password strength validator).
 * Quy tắc business tham khảo phổ biến: cần có chữ thường, chữ hoa và chữ số.
 * @returns ValidatorFn (hàm validator) để gắn vào FormControl.
 */
export function passwordStrengthValidator(): ValidatorFn {
  /**
   * Trả về hàm validator được Angular gọi khi giá trị control thay đổi.
   * @param control AbstractControl (điều khiển form) chứa value hiện tại.
   * @returns ValidationErrors hoặc null nếu hợp lệ.
   */
  return (control: AbstractControl): ValidationErrors | null => {
    /**
     * Chuẩn hóa value: chỉ xử lý khi value là string.
     */
    const rawValue: unknown = control.value;
    const value: string = typeof rawValue === 'string' ? rawValue : '';

    /**
     * Nếu chưa nhập: không báo lỗi strength ở đây, để required xử lý.
     */
    if (!value) {
      return null;
    }

    /**
     * Kiểm tra chữ thường (lowercase).
     */
    const hasLowerCase: boolean = /[a-z]/.test(value);

    /**
     * Kiểm tra chữ hoa (uppercase).
     */
    const hasUpperCase: boolean = /[A-Z]/.test(value);

    /**
     * Kiểm tra chữ số (number).
     */
    const hasNumber: boolean = /\d/.test(value);

    /**
     * Nếu thiếu bất kỳ điều kiện nào: trả về lỗi passwordStrength kèm chi tiết.
     */
    if (!hasLowerCase || !hasUpperCase || !hasNumber) {
      return {
        passwordStrength: {
          hasLowerCase,
          hasUpperCase,
          hasNumber,
        },
      };
    }

    /**
     * Hợp lệ: không có lỗi.
     */
    return null;
  };
}
