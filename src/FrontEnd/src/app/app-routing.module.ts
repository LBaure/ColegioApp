import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './layouts/not-found/not-found.component';
import { PageBadGatewayComponent } from './layouts/page-bad-gateway/page-bad-gateway.component';
import { environment } from 'src/environments/environment';
import { RedirectionComponent } from './redirection/redirection.component';
import { RedirectToComponent } from './redirect-to/redirect-to.component';
import { TwoFactorAuthComponent } from './expedientes-web/components/two-factor-auth/two-factor-auth.component';
import { AuthGuardService } from './utils/auth-guard.service';
import { PageNotFoundComponent } from './layouts/page-not-found/page-not-found.component';
import { PageErrorComponent } from './layouts/page-error/page-error.component';
import { PublicAppComponent } from './layouts/public-app/public-app.component';
import { LoginComponent } from './layouts/login/login.component';

const routes: Routes = [

  // {
  //   path: 'login',
  //   component: RedirectionComponent,
  //   resolve: { url: 'externalUrlRedirectResolver' },
  //   data: { externalUrl: environment.ssoLogin }
  // },
  {
    path: '',
    component: RedirectToComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'public',
    component: PublicAppComponent,
    loadChildren: () => import('./public-app/public-app.module').then(m => m.PublicAppModule)
  },
  {
    path: 'ew',
    loadChildren: () => import('./expedientes-web/expedientes-web.module').then(m => m.ExpedientesWebModule)
  },
  {
    path: 'off-line',
    component: PageBadGatewayComponent
  },
  {
    path: 'two-factor-auth',
    component: TwoFactorAuthComponent,
    canActivate: [AuthGuardService],
    canLoad: [AuthGuardService],
    canActivateChild: [AuthGuardService]
  },
  {
    path: 'not-found',
    component: PageNotFoundComponent
  },
  {
    path: '404',
    component: PageErrorComponent
  },

  {
    path: '**',
    component: PageNotFoundComponent
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
