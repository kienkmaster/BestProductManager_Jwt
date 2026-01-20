import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validator kiểm tra giá trị không được chỉ chứa khoảng trắng.
 * @returns ValidatorFn trả về lỗi noWhitespace nếu chuỗi rỗng sau khi trim.
 */
export function noWhitespaceValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    // Nếu control chưa có giá trị thì không kết luận lỗi ở đây (để required xử lý).
    if (control.value === null || control.value === undefined) {
      return null;
    }

    // Chuẩn hóa về string và kiểm tra sau khi trim.
    const value = String(control.value);

    // Nếu người dùng chỉ nhập khoảng trắng thì báo lỗi.
    if (value.trim().length === 0) {
      return { noWhitespace: true };
    }

    return null;
  };
}
