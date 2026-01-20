import { AbstractControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

/**
 * Tạo danh sách validators (bộ kiểm tra) cho trường userName của login.
 * @returns Mảng ValidatorFn dùng cho FormControl userName.
 */
export function buildLoginUserNameValidators(): ValidatorFn[] {
  // Gom các validator theo chuẩn business để tái sử dụng, tránh hard-code trong component.
  return [
    Validators.required,
    Validators.maxLength(50),
    noWhitespaceValidator(),
  ];
}

/**
 * Tạo danh sách validators (bộ kiểm tra) cho trường password của login.
 * @returns Mảng ValidatorFn dùng cho FormControl password.
 */
export function buildLoginPasswordValidators(): ValidatorFn[] {
  // Gom các validator theo chuẩn business để tái sử dụng, tránh hard-code trong component.
  return [
    Validators.required,
    Validators.minLength(6),
    Validators.maxLength(20),
    noWhitespaceValidator(),
  ];
}

/**
 * Validator kiểm tra không cho phép khoảng trắng và không cho phép chuỗi chỉ toàn khoảng trắng.
 * @returns ValidatorFn trả về lỗi khi dữ liệu chứa khoảng trắng hoặc chỉ toàn khoảng trắng.
 */
export function noWhitespaceValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    // Bỏ qua khi chưa có giá trị.
    if (control.value === null || control.value === undefined) {
      return null;
    }

    // Chuẩn hóa giá trị về string để kiểm tra.
    const raw = String(control.value);

    // Không cho phép chuỗi rỗng sau trim.
    if (raw.trim().length === 0) {
      return { whitespaceNotAllowed: true };
    }

    // Không cho phép chứa khoảng trắng bên trong.
    if (/\s/.test(raw)) {
      return { whitespaceNotAllowed: true };
    }

    return null;
  };
}
