import { NgModule } from '@angular/core';
import { ExpedientesWebLayoutComponent } from '../layouts/expedientes-web-layout/expedientes-web-layout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuardService } from '../utils/auth-guard.service';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: ExpedientesWebLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: '/ew/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuardService],
        canLoad: [AuthGuardService],
        canActivateChild: [AuthGuardService]
      },
      {
        path: 'reportes',
        loadChildren: () => import('./modules/reportes/reportes.module').then(m => m.ReportesModule)
      },
      {
        path: 'seguridad',
        loadChildren: () => import('./modules/seguridad/seguridad.module').then(m => m.SeguridadModule)
      },
      {
        path: 'admin',
        loadChildren: () => import('./modules/administracion/administracion.module').then(m => m.AdministracionModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExpedientesWebRoutingModule { }
