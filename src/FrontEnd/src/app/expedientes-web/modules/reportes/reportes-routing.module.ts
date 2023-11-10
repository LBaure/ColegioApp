import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExpedientesComponent } from './expedientes/expedientes.component';
import { FaseExpedienteComponent } from './fase-expediente/fase-expediente.component';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
  {
    path: 'expedientes',
    component: ExpedientesComponent
  },
  {
    path: 'fase-expediente/:tipo-expediente',
    component: FaseExpedienteComponent
  },
  {
    path: 'fase-expediente',
    component: FaseExpedienteComponent
  },
  {
    path: 'dashboard',
    component: DashboardComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportesRoutingModule {

 }
