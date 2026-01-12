import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgIf, NgForOf, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../shared/models/product.model';
import { AuthService } from '../../core/services/auth.service';

/**
 * Màn hình hiển thị danh sách sản phẩm.
 * - Yêu cầu người dùng đã đăng nhập (cookie) mới gọi được API sản phẩm.
 * - Hỗ trợ tìm kiếm sản phẩm theo Id hoặc tên.
 * - Hỗ trợ đăng ký mới, cập nhật và xóa sản phẩm.
 */
@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    NgIf,
    NgForOf,
    DecimalPipe,
    FormsModule
  ],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  /** Danh sách sản phẩm hiển thị trên bảng. */
  products: Product[] = [];

  /** Thông báo lỗi khi tải danh sách sản phẩm hoặc thao tác với API. */
  errorMessage: string | null = null;

  /** Từ khóa tìm kiếm sản phẩm (Id hoặc tên sản phẩm). */
  searchKeyword = '';

  /** Thông báo cảnh báo liên quan đến chức năng tìm kiếm. */
  searchMessage: string | null = null;

  /** Cờ cho biết form đang ở chế độ đăng ký mới (true) hay cập nhật (false). */
  isCreateMode = false;

  /** Id sản phẩm đang được chọn để cập nhật (null nếu không chọn hoặc đang đăng ký mới). */
  selectedProductId: number | null = null;

  /** Tên sản phẩm trên form Cập nhật / Đăng ký sản phẩm. */
  editProductName = '';

  /** Đơn giá trên form Cập nhật / Đăng ký sản phẩm. */
  editPrice: number | null = null;

  /** Số lượng lưu kho trên form Cập nhật / Đăng ký sản phẩm. */
  editStock: number | null = null;

  /** Thông báo lỗi hiển thị tại layout Cập nhật / Đăng ký sản phẩm. */
  updateErrorMessage: string | null = null;

  /** Tham chiếu tới textbox tìm kiếm để xử lý focus. */
  @ViewChild('searchInput') searchInput?: ElementRef<HTMLInputElement>;

  /**
   * Khởi tạo ProductListComponent với ProductService và AuthService.
   * @param productService Dịch vụ dùng để gọi API sản phẩm.
   * @param authService Dịch vụ xác thực để kiểm tra trạng thái đăng nhập.
   */
  constructor(
    private productService: ProductService,
    public authService: AuthService
  ) { }

  /**
   * Khi component khởi tạo:
   * - Nếu người dùng đã đăng nhập thì gọi API lấy danh sách sản phẩm.
   * - Nếu chưa đăng nhập thì hiển thị thông báo yêu cầu đăng nhập.
   */
  async ngOnInit(): Promise<void> {
    this.errorMessage = null;
    this.searchMessage = null;

    if (!this.authService.isLoggedIn) {
      return;
    }

    await this.loadAllProducts();
  }

  /**
   * Tải toàn bộ danh sách sản phẩm từ API và gán vào thuộc tính products.
   */
  private async loadAllProducts(): Promise<void> {
    this.errorMessage = null;

    try {
      this.products = await this.productService.getProducts();
    }
    catch {
      this.errorMessage = 'Không thể tải danh sách sản phẩm từ API.';
    }
  }

  /**
   * Xử lý tìm kiếm khi người dùng nhấn Enter hoặc click nút "Tìm".
   * Tìm theo Id nếu nhập số, nếu không thì tìm tương đối theo tên sản phẩm.
   */
  async onSearch(): Promise<void> {
    this.errorMessage = null;
    this.searchMessage = null;
    this.updateErrorMessage = null;

    const keyword = this.searchKeyword?.trim() ?? '';

    if (!keyword) {
      this.searchMessage = 'Vui lòng nhập Id hoặc tên sản phẩm.';
      this.focusSearchInput();
      return;
    }

    try {
      const result = await this.productService.searchProducts(keyword);

      if (!result || result.length === 0) {
        this.products = [];
        this.searchMessage = 'Không tìm thấy sản phẩm liên quan.';
        this.focusSearchInput();
        return;
      }

      this.products = result;
    }
    catch {
      this.errorMessage = 'Không thể tìm kiếm sản phẩm từ API.';
    }
  }

  /**
   * Hiển thị lại toàn bộ danh sách sản phẩm khi người dùng click "Hiển thị tất cả".
   * Đồng thời reset trạng thái form đăng ký/cập nhật.
   */
  async onShowAll(): Promise<void> {
    this.searchKeyword = '';
    this.searchMessage = null;
    this.updateErrorMessage = null;

    this.resetEditForm();

    await this.loadAllProducts();
  }

  /**
   * Chuyển form sang chế độ đăng ký sản phẩm mới và reset dữ liệu form.
   */
  onStartCreate(): void {
    this.isCreateMode = true;
    this.selectedProductId = null;
    this.editProductName = '';
    this.editPrice = null;
    this.editStock = null;
    this.updateErrorMessage = null;
  }

  /**
   * Chọn một dòng sản phẩm để hiển thị lên layout Cập nhật sản phẩm.
   * @param product Sản phẩm được chọn trong grid.
   */
  onSelectProduct(product: Product): void {
    this.isCreateMode = false;
    this.selectedProductId = product.id;
    this.editProductName = product.productName;
    this.editPrice = product.price;
    this.editStock = product.stock;
    this.updateErrorMessage = null;
  }

  /**
   * Thực hiện gọi API cập nhật thông tin sản phẩm hiện tại.
   * Sử dụng các giá trị trên form Cập nhật sản phẩm.
   */
  async onUpdateProduct(): Promise<void> {
    this.updateErrorMessage = null;
    this.errorMessage = null;

    if (this.selectedProductId === null) {
      this.updateErrorMessage = 'Không xác định được sản phẩm cần cập nhật.';
      return;
    }

    const name = this.editProductName?.trim() ?? '';
    const price = this.editPrice;
    const stock = this.editStock;

    if (!name || price == null || Number.isNaN(price) || stock == null || Number.isNaN(stock)) {
      this.updateErrorMessage =
        'Vui lòng nhập đầy đủ Tên sản phẩm, Đơn giá và Lưu kho hợp lệ.';
      return;
    }

    const productToUpdate: Product = {
      id: this.selectedProductId,
      productName: name,
      price: Number(price),
      stock: Number(stock)
    };

    try {
      await this.productService.updateProduct(productToUpdate);

      const index = this.products.findIndex(p => p.id === productToUpdate.id);
      if (index >= 0) {
        this.products[index] = { ...productToUpdate };
      }

      this.resetEditForm();
    }
    catch {
      this.updateErrorMessage = 'Cập nhật sản phẩm không thành công.';
    }
  }

  /**
   * Thực hiện gọi API đăng ký (tạo mới) sản phẩm sử dụng dữ liệu trên form.
   */
  async onCreateProduct(): Promise<void> {
    this.updateErrorMessage = null;
    this.errorMessage = null;

    const name = this.editProductName?.trim() ?? '';
    const price = this.editPrice;
    const stock = this.editStock;

    if (!name || price == null || Number.isNaN(price) || stock == null || Number.isNaN(stock)) {
      this.updateErrorMessage =
        'Vui lòng nhập đầy đủ Tên sản phẩm, Đơn giá và Lưu kho hợp lệ.';
      return;
    }

    try {
      await this.productService.createProduct({
        productName: name,
        price: Number(price),
        stock: Number(stock)
      });

      await this.loadAllProducts();
      this.resetEditForm();
    }
    catch {
      this.updateErrorMessage = 'Đăng ký sản phẩm không thành công.';
    }
  }

  /**
   * Thực hiện xóa một sản phẩm sau khi người dùng click link Delete.
   * @param product Sản phẩm cần xóa.
   */
  async onDelete(product: Product): Promise<void> {
    this.errorMessage = null;
    this.searchMessage = null;
    this.updateErrorMessage = null;

    try {
      await this.productService.deleteProduct(product.id);

      if (this.searchKeyword?.trim()) {
        await this.onSearch();
      }
      else {
        await this.loadAllProducts();
      }

      if (this.selectedProductId === product.id) {
        this.resetEditForm();
      }
    }
    catch {
      this.errorMessage = 'Không thể xóa sản phẩm từ API.';
    }
  }

  /**
   * Đưa focus vào ô nhập từ khóa tìm kiếm nếu tham chiếu tồn tại.
   */
  private focusSearchInput(): void {
    if (this.searchInput?.nativeElement) {
      this.searchInput.nativeElement.focus();
      this.searchInput.nativeElement.select();
    }
  }

  /**
   * Reset trạng thái form Cập nhật / Đăng ký sản phẩm về mặc định.
   */
  private resetEditForm(): void {
    this.isCreateMode = false;
    this.selectedProductId = null;
    this.editProductName = '';
    this.editPrice = null;
    this.editStock = null;
    this.updateErrorMessage = null;
  }
}
