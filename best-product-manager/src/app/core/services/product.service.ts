import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Product } from '../../shared/models/product.model';
import { environment } from '../../../environments/environment';

interface productResult {
  data: Product[];
  message?: string;
  success?: boolean;
}

/**
 * Dịch vụ quản lý dữ liệu sản phẩm (ProductService).
 * Chịu trách nhiệm gọi API BestProductManager để:
 * - Lấy danh sách sản phẩm.
 * - Tìm kiếm sản phẩm.
 * - Đăng ký (tạo mới) sản phẩm.
 * - Cập nhật sản phẩm.
 * - Xóa sản phẩm.
 */
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  /**
   * Địa chỉ gốc của API BestProductManager.
   * Cần trùng với URL profile trong cấu hình publish của API.
   */
  private readonly apiBaseUrl = `http://localhost/${environment.ApiTarget}/api`;

  /**
   * Khởi tạo ProductService với HttpClient.
   * @param http HttpClient dùng để gọi API.
   */
  constructor(private http: HttpClient) { }

  /**
   * Gọi API /api/Products/getallproduct để lấy danh sách sản phẩm.
   * Yêu cầu: user đã đăng nhập, cookie xác thực sẽ được gửi kèm theo request.
   * @returns Danh sách Product lấy từ API.
   */
  async getProducts(): Promise<Product[]> {
    const result = await firstValueFrom(
      this.http.get<productResult>(
        `${this.apiBaseUrl}/Products/getallproduct`,
        {
          withCredentials: true
        }
      )
    );

    return result.data ?? [];
  }

  /**
   * Tìm kiếm sản phẩm theo Id hoặc tên sản phẩm.
   * Mapping tới API /api/Products/searchproduct.
   * @param keyword Giá trị nhập từ người dùng (Id hoặc tên sản phẩm cần tìm).
   * @returns Danh sách sản phẩm phù hợp điều kiện tìm kiếm.
   */
  async searchProducts(keyword: string): Promise<Product[]> {
    const trimmedKeyword = keyword?.trim() ?? '';

    // Nếu từ khóa rỗng, trả về danh sách rỗng để component tự xử lý cảnh báo.
    if (!trimmedKeyword) {
      return [];
    }

    try {
      const result = await firstValueFrom(
        this.http.get<productResult>(
          `${this.apiBaseUrl}/Products/searchproduct`,
          {
            params: {
              keyword: trimmedKeyword
            },
            withCredentials: true
          }
        )
      );

      return result.data ?? [];
    }
    catch {
      // Trường hợp lỗi (ví dụ API trả 404), trả về danh sách rỗng để component xử lý.
      return [];
    }
  }

  /**
   * Đăng ký (tạo mới) một sản phẩm.
   * Mapping tới API POST /api/Products/create.
   * @param product Thông tin sản phẩm cần đăng ký (không bao gồm Id).
   * @returns Id mới của sản phẩm được tạo (nếu API trả về).
   */
  async createProduct(product: Omit<Product, 'id'>): Promise<number> {
    const payload = {
      productName: product.productName,
      price: product.price,
      stock: product.stock
    };

    const result = await firstValueFrom(
      this.http.post<{ success?: boolean; newId?: number; message?: string }>(
        `${this.apiBaseUrl}/Products/create`,
        payload,
        {
          withCredentials: true
        }
      )
    );

    return result.newId ?? 0;
  }

  /**
   * Cập nhật thông tin một sản phẩm cụ thể.
   * Mapping tới API PUT /api/Products/update/{id}.
   * @param product Đối tượng Product cần cập nhật.
   */
  async updateProduct(product: Product): Promise<void> {
    const payload = {
      productName: product.productName,
      price: product.price,
      stock: product.stock
    };

    await firstValueFrom(
      this.http.put(
        `${this.apiBaseUrl}/Products/update/${product.id}`,
        payload,
        {
          withCredentials: true
        }
      )
    );
  }

  /**
   * Xóa một sản phẩm theo Id.
   * Mapping tới API DELETE /api/Products/detete/{id}.
   * @param id Khóa chính của sản phẩm cần xóa.
   */
  async deleteProduct(id: number): Promise<void> {
    await firstValueFrom(
      this.http.delete(
        `${this.apiBaseUrl}/Products/detete/${id}`,
        {
          withCredentials: true
        }
      )
    );
  }
}
