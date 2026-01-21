/**
 * Cấu hình môi trường (environment configuration) cho production.
 * Lưu ý: apiBaseUrl cần được cấu hình phù hợp với môi trường production thực tế.
 * Nếu để rỗng, API calls sẽ sử dụng relative URLs (cùng domain với frontend).
 */
export const environment = {
  production: true,
  apiBaseUrl: '',
};
