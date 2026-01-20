import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { RegisterUserModel } from '../../models/register-user.model';

import { ApiMessageResponseModel } from '../../models/api-message-response.model';

import { API_ENDPOINTS } from './configs/api-endpoints';

@Injectable({
  /**
   * Cấu hình provider theo root injector để service dùng toàn ứng dụng.
   */
  providedIn: 'root',
})
export class UserRegisterService {
  /**
   * Khởi tạo service đăng ký (UserRegisterService).
   * @param http HttpClient (giao tiếp HTTP) dùng để gọi API.
   */
  constructor(
    private readonly http: HttpClient
  ) {
    /**
     * Service tập trung xử lý gọi API, không chứa UI logic.
     */
  }

  /**
   * Gọi API đăng ký user (register user).
   * @param model RegisterUserModel chứa userName/password/confirmPassword.
   * @returns Observable<ApiMessageResponseModel> để component subscribe và xử lý kết quả.
   */
  public registerAsync(model: RegisterUserModel): Observable<ApiMessageResponseModel> {
    /**
     * Thực hiện POST (HTTP) tới endpoint đăng ký đã quản lý tập trung.
     */
    return this.http.post<ApiMessageResponseModel>(
      API_ENDPOINTS.account.register,
      model
    );
  }
}
