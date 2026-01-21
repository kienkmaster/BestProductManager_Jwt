import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MaterialModule } from './shared/material/material.module';
import { HeaderComponent } from './components/shared/header/header.component';
import { UserRegisterComponent } from './components/user-register/user-register.component';
import { UserAuthComponent } from './components/user-auth/user-auth.component';
import { UserInfoComponent } from './components/user-info/user-info.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    UserRegisterComponent,
    UserAuthComponent,
    UserInfoComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule,
    MaterialModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
