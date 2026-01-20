/**
 * BreadcrumbItemModel (mô hình item breadcrumb) dùng để hiển thị navigator theo Router (Định tuyến).
 */
export class BreadcrumbItemModel {
    /**
     * Khởi tạo BreadcrumbItemModel.
     * @param label Nhãn hiển thị (label) của breadcrumb.
     * @param route Route (đường dẫn) để điều hướng; null nghĩa là item hiện tại (không click).
     */
    constructor(
      public readonly label: string,
      public readonly route: string | null
    ) {}
  }
