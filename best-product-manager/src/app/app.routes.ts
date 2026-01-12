import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ContactComponent } from './pages/contact/contact.component';
import { RegisterComponent } from './auth/register/register.component';
import { ProductListComponent } from './products/product-list/product-list.component';
import { MemberManagementComponent } from './admin/member-management/member-management.component';
import { ChangePasswordComponent } from './auth/change/change-password.component';
import { AdminChangePasswordComponent } from './admin/change/admin-change-password.component';
import { AdminChangeRoleComponent } from './admin/change-role/admin-change-role.component';

/**
 * Cấu hình định tuyến (routing configuration) cho ứng dụng BestProductManager.
 * Mỗi route ánh xạ một URL path đến một component standalone tương ứng.
 */
export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    title: 'Best Product Manager - Trang chủ'
  },
  {
    path: 'home',
    redirectTo: '',
    pathMatch: 'full'
  },
  {
    path: 'contact',
    component: ContactComponent,
    title: 'Best Product Manager - Liên hệ'
  },
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Best Product Manager - Đăng ký'
  },
  {
    path: 'products',
    component: ProductListComponent,
    title: 'Best Product Manager - Sản phẩm'
  },
  /**
   * Màn quản lý thành viên (chỉ hiển thị menu cho user có role Admin).
   */
  {
    path: 'admin/members',
    component: MemberManagementComponent,
    title: 'Best Product Manager - Quản lý thành viên'
  },
  /**
   * Màn đổi mật khẩu cho chính user đang đăng nhập.
   */
  {
    path: 'change-password',
    component: ChangePasswordComponent,
    title: 'Best Product Manager - Đổi mật khẩu'
  },
  /**
   * Màn đổi mật khẩu do admin sử dụng để đổi mật khẩu của thành viên khác.
   * URL chứa tham số userId để xác định thành viên mục tiêu.
   */
  {
    path: 'admin/change-password/:userId',
    component: AdminChangePasswordComponent,
    title: 'Best Product Manager - Đổi mật khẩu (Admin)'
  },
  /**
   * Màn thay đổi phân loại thành viên do admin sử dụng.
   * URL chứa tham số userId để xác định thành viên mục tiêu.
   */
  {
    path: 'admin/change-role/:userId',
    component: AdminChangeRoleComponent,
    title: 'Best Product Manager - Thay đổi phân loại thành viên'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
