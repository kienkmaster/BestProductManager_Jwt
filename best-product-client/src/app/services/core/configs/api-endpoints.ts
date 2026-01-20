import { environment } from '../../../../environments/environment';

/**
 * Chuẩn hóa baseUrl (normalize base URL) để tránh lỗi nối chuỗi khi có hoặc không có dấu '/' ở cuối.
 * @param baseUrl Base URL (đường dẫn gốc) cần chuẩn hóa.
 * @returns Base URL đã chuẩn hóa (không có dấu '/' ở cuối).
 */
function normalizeBaseUrl(baseUrl: string): string {
  // Trường hợp baseUrl rỗng: trả về rỗng để tránh tạo endpoint sai.
  if (!baseUrl) {
    return '';
  }

  // Trường hợp baseUrl kết thúc bằng '/': cắt bỏ ký tự '/' cuối để thống nhất.
  if (baseUrl.endsWith('/')) {
    return baseUrl.substring(0, baseUrl.length - 1);
  }

  // Trường hợp baseUrl đã chuẩn: trả về 그대로.
  return baseUrl;
}

/**
 * Base URL (đường dẫn gốc) lấy từ environment và chuẩn hóa.
 */
const baseUrl: string = normalizeBaseUrl(environment.apiBaseUrl);

/**
 * Danh sách endpoint (API endpoints) theo nhóm nghiệp vụ.
 * Mục tiêu: tập trung quản lý đường dẫn API, tránh hard-code rải rác trong service/component.
 */
export const API_ENDPOINTS = {
  account: {
    /**
     * Endpoint đăng ký (register).
     */
    register: `${baseUrl}/Account/register`,

    /**
     * Endpoint đăng nhập (login).
     */
    login: `${baseUrl}/Account/Login`,

    /**
     * Endpoint lấy user hiện tại (current user / me).
     */
    me: `${baseUrl}/Account/me`,
  },
};
