import { Component } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss'],
  standalone: false
})
export class SpinnerComponent {
  /**
   * Spinner (vòng quay tải) dùng để hiển thị trạng thái đang xử lý bất đồng bộ.
   * Component này chỉ chịu trách nhiệm hiển thị UI, không chứa xử lý nghiệp vụ.
   */
}
