import { Component, EventEmitter, Input, Output } from '@angular/core';

import { BreadcrumbItemModel } from '../../../models/breadcrumb-item.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false
})
export class HeaderComponent {
  /**
   * Tiêu đề ứng dụng hiển thị trên thanh header.
   */
  @Input()
  public appTitle = '';

  /**
   * Trạng thái đã xác thực (authentication state) để quyết định menu.
   */
  @Input()
  public isAuthenticated = false;

  /**
   * Tên user hiện tại để hiển thị khi đã đăng nhập.
   */
  @Input()
  public currentUserName = '';

  /**
   * Danh sách breadcrumb để hiển thị navigator theo Router (Định tuyến).
   */
  @Input()
  public breadcrumbs: BreadcrumbItemModel[] = [];

  /**
   * Sự kiện click menu "Đăng ký tài khoản".
   */
  @Output()
  public readonly registerClick = new EventEmitter<void>();

  /**
   * Sự kiện click menu "Đăng nhập".
   */
  @Output()
  public readonly loginClick = new EventEmitter<void>();

  /**
   * Sự kiện click menu "Đăng xuất".
   */
  @Output()
  public readonly logoutClick = new EventEmitter<void>();

  /**
   * Sự kiện click link tên user.
   */
  @Output()
  public readonly currentUserClick = new EventEmitter<void>();

  /**
   * Sự kiện điều hướng theo breadcrumb (route string).
   */
  @Output()
  public readonly breadcrumbNavigate = new EventEmitter<string>();

  /**
   * User click menu "Đăng ký tài khoản".
   * @returns void
   */
  public handleRegisterClick(): void {
    // Emit sự kiện để AppComponent điều phối hiển thị trang register.
    this.registerClick.emit();
  }

  /**
   * User click menu "Đăng nhập".
   * @returns void
   */
  public handleLoginClick(): void {
    // Emit sự kiện để AppComponent điều phối hiển thị trang login.
    this.loginClick.emit();
  }

  /**
   * User click menu "Đăng xuất".
   * @returns void
   */
  public handleLogoutClick(): void {
    // Emit sự kiện để AppComponent thực hiện logout và điều phối UI.
    this.logoutClick.emit();
  }

  /**
   * User click link tên user (đã đăng nhập).
   * @returns void
   */
  public handleCurrentUserClick(): void {
    // Emit sự kiện để AppComponent kích hoạt điều hướng và load thông tin user.
    this.currentUserClick.emit();
  }

  /**
   * User click breadcrumb item để điều hướng theo route.
   * @param route Route (đường dẫn) cần điều hướng.
   * @returns void
   */
  public handleBreadcrumbNavigate(route: string): void {
    // Emit route để AppComponent điều phối Router.navigate.
    this.breadcrumbNavigate.emit(route);
  }
}
