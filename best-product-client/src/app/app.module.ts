import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { HeaderComponent } from './components/shared/header/header.component';
import { UserRegisterComponent } from './components/user-register/user-register.component';
import { UserAuthComponent } from './components/user-auth/user-auth.component';
import { UserInfoComponent } from './components/user-info/user-info.component';

import { SpinnerComponent } from './shared/ui/spinner/spinner.component';
import { MaterialModule } from './shared/material/material.module';

import { USER_REGISTER_FEATURE_KEY } from './stores/user-register/user-register.store';
import { userRegisterReducer } from './stores/user-register/user-register.reducer';
import { UserRegisterEffects } from './stores/user-register/user-register.effects';

import { AUTH_FEATURE_KEY } from './stores/auth/auth.store';
import { authReducer } from './stores/auth/auth.reducer';
import { AuthEffects } from './stores/auth/auth.effects';

import { LOADING_FEATURE_KEY } from './stores/loading/loading.store';
import { loadingReducer } from './stores/loading/loading.reducer';

import { LoadingInterceptor } from './services/core/http-interceptors/loading.interceptor';
import { AuthInterceptor } from './services/core/http-interceptors/auth.interceptor';
import { ErrorInterceptor } from './services/core/http-interceptors/error.interceptor';

@NgModule({
  /**
   * Khai báo (declarations) các component thuộc module.
   */
  declarations: [
    AppComponent,
    HeaderComponent,
    UserRegisterComponent,
    UserAuthComponent,
    UserInfoComponent,
    SpinnerComponent,
  ],
  /**
   * Import (imports) các module nền tảng và module UI/form/http/state.
   */
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule,
    MaterialModule,

    // Đăng ký NgRx Store (StoreModule) root runtime.
    StoreModule.forRoot({}),

    // Đăng ký NgRx Store (StoreModule) cho feature đăng ký user.
    StoreModule.forFeature(USER_REGISTER_FEATURE_KEY, userRegisterReducer),

    // Đăng ký NgRx Store (StoreModule) cho feature đăng nhập + current user.
    StoreModule.forFeature(AUTH_FEATURE_KEY, authReducer),

    // Đăng ký NgRx Store (StoreModule) cho feature loading toàn cục.
    StoreModule.forFeature(LOADING_FEATURE_KEY, loadingReducer),

    // Đăng ký NgRx Effects (EffectsModule) để xử lý gọi API đăng ký và đăng nhập.
    EffectsModule.forRoot([
      UserRegisterEffects,
      AuthEffects,
    ]),
  ],
  /**
   * Providers (DI providers) cho HTTP interceptors.
   */
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
  ],
  /**
   * Bootstrap (khởi chạy) root component của ứng dụng.
   */
  bootstrap: [
    AppComponent,
  ],
})
export class AppModule {
  /**
   * Module gốc của ứng dụng (root module).
   */
}
