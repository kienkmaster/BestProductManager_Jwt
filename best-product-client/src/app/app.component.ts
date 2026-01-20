import { Component } from '@angular/core';

import { NavigationEnd, Router } from '@angular/router';

import { Store } from '@ngrx/store';

import { Observable, filter } from 'rxjs';

import { BreadcrumbItemModel } from './models/breadcrumb-item.model';

import { authLoadCurrentUser, authLogout } from './stores/auth/auth.actions';

import { selectIsLoading } from './stores/loading/loading.selector';

type CurrentPage = 'home' | 'register' | 'login';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent {
  /**
   * Tiêu đề hiển thị trên header.
   */
  public readonly appTitle = 'Best Product Manager';

  /**
   * Trạng thái trang hiện tại (giữ nguyên cơ chế hiện có cho home/register/login).
   */
  public currentPage: CurrentPage = 'home';

  /**
   * Trạng thái đã xác thực để điều khiển menu.
   */
  public isAuthenticated = false;

  /**
   * Tên user hiện tại để hiển thị trên menu.
   */
  public currentUserName = '';

  /**
   * Observable trạng thái loading toàn cục để hiển thị spinner overlay.
   */
  public readonly isLoading$: Observable<boolean>;

  /**
   * Breadcrumb items để HeaderComponent hiển thị navigator.
   */
  public breadcrumbs: BreadcrumbItemModel[] = [];

  /**
   * Cờ xác định đang ở route user-info hay không.
   */
  public isUserInfoRoute = false;

  /**
   * Khởi tạo AppComponent.
   * @param store NgRx Store dùng để dispatch action/select state.
   * @param router Router dùng để điều hướng user-info theo URL.
   */
  constructor(
    private readonly store: Store,
    private readonly router: Router
  ) {
    // Khởi tạo observable sau khi DI hoàn tất.
    this.isLoading$ = this.store.select(selectIsLoading);

    // Debug: theo dõi isLoading$ để xác nhận state đang bật/tắt.
    this.isLoading$.subscribe((value: boolean) => {
      // Đặt breakpoint ở đây để xem value true/false.
      console.log('isLoading =', value);
    });

    // Đồng bộ trạng thái route ngay khi app khởi tạo.
    this.updateRouteState(this.router.url);

    // Lắng nghe thay đổi route để cập nhật breadcrumb và cờ isUserInfoRoute.
    this.router.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd)
      )
      .subscribe((event: NavigationEnd) => {
        // Cập nhật route state theo URL sau điều hướng.
        this.updateRouteState(event.urlAfterRedirects);
      });

    // Khởi tạo breadcrumb theo trạng thái hiện tại.
    this.refreshBreadcrumbs();
  }

  /**
   * User click breadcrumb để điều hướng theo route.
   * @param route Route cần điều hướng.
   * @returns void
   */
  public handleBreadcrumbNavigate(route: string): void {
    // Điều hướng theo Router để URL phản ánh đúng trạng thái UI.
    this.router.navigateByUrl(route);

    // Khi về trang chủ, duy trì cơ chế page hiện có để không ảnh hưởng tính năng khác.
    if (route === '/' || route === '') {
      this.currentPage = 'home';
    }

    // Refresh breadcrumb sau khi điều phối.
    this.refreshBreadcrumbs();
  }

  /**
   * Điều phối UI về trang chủ theo cơ chế hiện tại.
   * @returns void
   */
  public handleHomeNavigateClick(): void {
    // Set page về home để hiển thị nội dung trang chủ.
    this.currentPage = 'home';

    // Điều hướng về root để đồng bộ URL (hướng Router).
    this.router.navigateByUrl('/');

    // Refresh breadcrumb sau khi điều phối.
    this.refreshBreadcrumbs();
  }

  /**
   * Điều phối UI sang trang đăng ký theo cơ chế hiện tại.
   * @returns void
   */
  public handleRegisterClick(): void {
    // Set page về register để hiển thị form đăng ký.
    this.currentPage = 'register';

    // Refresh breadcrumb theo page hiện tại.
    this.refreshBreadcrumbs();
  }

  /**
   * Điều phối UI sang trang đăng nhập theo cơ chế hiện tại.
   * @returns void
   */
  public handleLoginClick(): void {
    // Set page về login để hiển thị form đăng nhập.
    this.currentPage = 'login';

    // Refresh breadcrumb theo page hiện tại.
    this.refreshBreadcrumbs();
  }

  /**
   * Xử lý logout khi user click menu "Đăng xuất".
   * @returns void
   */
  public handleLogoutClick(): void {
    // Dispatch logout để reset auth state trong NgRx.
    this.store.dispatch(authLogout());

    // Reset thông tin menu theo yêu cầu hiện trạng.
    this.isAuthenticated = false;
    this.currentUserName = '';

    // Điều phối UI về trang đăng nhập theo cơ chế hiện tại.
    this.currentPage = 'login';

    // Nếu đang ở user-info thì đưa về root để tránh giữ route cần auth.
    this.router.navigateByUrl('/');

    // Refresh breadcrumb sau khi điều phối.
    this.refreshBreadcrumbs();
  }

  /**
   * User click link tên user để hiển thị thông tin user (GET /Account/me) theo Router.
   * @returns void
   */
  public handleCurrentUserClick(): void {
    // Dispatch action để Effect gọi API /Account/me và lưu dữ liệu vào store.
    this.store.dispatch(authLoadCurrentUser());

    // Điều hướng sang route user-info để Router render UserInfoComponent.
    this.router.navigateByUrl('/user-info');
  }

  /**
   * Xử lý khi đăng ký thành công.
   * @param userName Tên user vừa đăng ký thành công.
   * @returns void
   */
  public handleRegisterSuccess(userName: string): void {
    // Theo yêu cầu hiện trạng: đăng ký thành công hiển thị status tại register và clear form.
  }

  /**
   * Xử lý khi đăng nhập thành công.
   * @param userName Tên user đã đăng nhập.
   * @returns void
   */
  public handleLoginSuccess(userName: string): void {
    // Cập nhật trạng thái menu sau login.
    this.isAuthenticated = true;
    this.currentUserName = userName;

    // Điều phối về trang chủ sau login.
    this.currentPage = 'home';

    // Điều hướng về root để URL phản ánh trạng thái home.
    this.router.navigateByUrl('/');

    // Refresh breadcrumb sau khi điều phối.
    this.refreshBreadcrumbs();
  }

  /**
   * Cập nhật trạng thái route nội bộ theo URL hiện tại.
   * @param url URL hiện tại của Router.
   * @returns void
   */
  private updateRouteState(url: string): void {
    // Xác định route user-info để AppComponent render router-outlet đúng vùng.
    this.isUserInfoRoute = url.startsWith('/user-info');

    // Refresh breadcrumb theo URL/page hiện tại.
    this.refreshBreadcrumbs();
  }

  /**
   * Refresh breadcrumb theo URL (Router) và currentPage (cơ chế hiện có).
   * @returns void
   */
  private refreshBreadcrumbs(): void {
    // Luôn có breadcrumb "Trang chủ" để điều hướng về root.
    const items: BreadcrumbItemModel[] = [
      new BreadcrumbItemModel('Trang chủ', '/'),
    ];

    // Khi route là user-info thì ưu tiên breadcrumb theo Router.
    if (this.isUserInfoRoute === true) {
      items.push(new BreadcrumbItemModel('Thông tin người dùng', null));
      this.breadcrumbs = items;
      return;
    }

    // Giữ nguyên breadcrumb theo cơ chế page hiện có cho register/login.
    if (this.currentPage === 'register') {
      items.push(new BreadcrumbItemModel('Đăng ký tài khoản', null));
    }

    if (this.currentPage === 'login') {
      items.push(new BreadcrumbItemModel('Đăng nhập', null));
    }

    // Cập nhật breadcrumbs để HeaderComponent hiển thị.
    this.breadcrumbs = items;
  }
}
