import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

/**
 * Điểm khởi động (bootstrap) của ứng dụng Angular.
 * Khởi chạy AppComponent với cấu hình ứng dụng appConfig.
 */
bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
