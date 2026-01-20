/**
 * Key của feature state (feature key) dùng để đăng ký reducer và select state.
 */
export const USER_REGISTER_FEATURE_KEY: string = 'userRegister';

/**
 * Trạng thái đăng ký user (UserRegisterState) quản lý bằng NgRx Store.
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
   * Component sẽ xử lý reset form và phát action "handled".
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
export const initialUserRegisterState: UserRegisterState = {
  isSubmitting: false,
  statusMessage: '',
  isErrorStatus: false,
  shouldResetForm: false,
  lastRegisteredUserName: null,
};
