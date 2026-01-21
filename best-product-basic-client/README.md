## Cấu trúc thư mục (dự kiến) khi chuyển quản lý state từ ngrx sang service

```text
best-product-basic-client/
└── src/
    └── app/
        ├── components/              # UI components
        │   ├── shared/
        │   ├── user-auth/
        │   ├── user-register/
        │   └── user-info/
        ├── services/
        │   ├── core/                # HTTP, auth, interceptors, cấu hình API
        │   └── state/               # Quản lý state bằng service
        │       ├── models/          # Interface/model cho state (trước đây trong store)
        │       ├── user/            # Service state cho user (vd: AuthStateService)
        │       ├── product/         # Service state cho product (vd: ProductStateService)
        │       └── shared/          # Base state service, helpers (vd: StateSubject)
        ├── models/                  # Model dùng chung (giữ nguyên vị trí hiện tại)
        ├── app-routing.module.ts
        ├── app.module.ts            # Bỏ import StoreModule/EffectsModule khi hoàn tất chuyển đổi
        ├── app.component.* 
        └── shared/                  # Module/material dùng chung
```

**Lưu ý:** Đây là cấu trúc dự kiến phục vụ chuyển đổi sang service-based state management. Các chức năng ngoài phạm vi chuyển đổi cần được giữ nguyên hoạt động.
