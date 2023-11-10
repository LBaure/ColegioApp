import { NgModule } from '@angular/core';
import { TwoFactorAuthComponent } from './components/two-factor-auth/two-factor-auth.component';
import { ValidarCredencialesComponent } from './components/validar-credenciales/validar-credenciales.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ResizeService } from '../utils/resize.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ExpedientesWebRoutingModule } from './expedientes-web-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../utils/auth.service';
import { AuthGuardService } from '../utils/auth-guard.service';
import { AuthInterceptor } from './utils/auth.interceptor';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { InputYearComponent } from './components/input-year/input-year.component';

@NgModule({
  declarations: [
    DashboardComponent,TwoFactorAuthComponent,
    ValidarCredencialesComponent,
    InputYearComponent
  ],
  imports: [
    CommonModule,
    ExpedientesWebRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgxMaskDirective,
    NgxMaskPipe,
    BsDatepickerModule.forRoot(),
  ],
  providers: [
    provideNgxMask(),
    ResizeService,
    AuthService,
    AuthGuardService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ]
})
export class ExpedientesWebModule { }
