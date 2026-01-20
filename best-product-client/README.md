ng new best-product-client --standalone=false --routing --style=scss --ssr=false --strict
cd best-product-client
ng add @angular/material --skip-confirmation
ng add @ngrx/store@19  --skip-confirmation
ng add @ngrx/effects@19 --skip-confirmation
ng add @ngrx/store-devtools@19 --skip-confirmation
npm install @ngrx/entity@19 @ngrx/router-store@19
npm install @angular/animations@19
ng g m shared/material --module app
ng g c components/shared/header --module app
ng g c components/user-register --module app
ng g c components/user-auth --module app
ng g c components/user-info --module app
ng g s services/core/user-register
ng g s services/core/user-auth
ng g interceptor services/core/http-interceptors/loading
ng g interceptor services/core/http-interceptors/auth
ng g interceptor services/core/http-interceptors/error
ng g class models/api-message-response --type=model
ng g class models/auth-user --type=model
ng g class models/breadcrumb-item --type=model
ng g class models/login-request --type=model
ng g class models/register-user --type=model
New-Item -ItemType Directory -Force -Path src\app\services\core\configs | Out-Null
New-Item -ItemType File -Force -Path src\app\services\core\configs\api-endpoints.ts | Out-Null
New-Item -ItemType Directory -Force -Path src\app\stores\auth | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\auth\auth.actions.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\auth\auth.effects.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\auth\auth.reducer.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\auth\auth.selector.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\auth\auth.store.ts | Out-Null
New-Item -ItemType Directory -Force -Path src\app\stores\loading | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\loading\loading.actions.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\loading\loading.reducer.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\loading\loading.selector.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\loading\loading.store.ts | Out-Null

New-Item -ItemType Directory -Force -Path src\app\stores\user-register | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\user-register\user-register.actions.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\user-register\user-register.effects.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\user-register\user-register.reducer.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\user-register\user-register.selector.ts | Out-Null
New-Item -ItemType File -Force -Path src\app\stores\user-register\user-register.store.ts | Out-Null

New-Item -ItemType File -Force -Path README.md | Out-Null
ng generate environments