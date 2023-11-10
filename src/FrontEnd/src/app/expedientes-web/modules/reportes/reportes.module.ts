import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportesRoutingModule } from './reportes-routing.module';
import { ExpedientesComponent } from './expedientes/expedientes.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { FaseExpedienteComponent } from './fase-expediente/fase-expediente.component';
import { ModalExpedientesInfoComponent } from './components/modal-expedientes-info/modal-expedientes-info.component';
import { ModalTipoExpedientesComponent } from './components/modal-tipo-expedientes/modal-tipo-expedientes.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ComponentesHtmlModule } from 'src/app/componentes-html/componentes-html.module';
import { ModalFiltrarFasesComponent } from './components/modal-filtrar-fases/modal-filtrar-fases.component';
import { ModalFiltrosAvanzadosComponent } from './components/modal-filtros-avanzados/modal-filtros-avanzados.component';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { DashboardFasesComponent } from './components/dashboard-fases/dashboard-fases.component';
import { DashboardFasesLateralComponent } from './components/dashboard-fases-lateral/dashboard-fases-lateral.component';

import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { TimelineComponent } from './components/timeline/timeline.component';
import { esLocale } from 'ngx-bootstrap/locale';
import { defineLocale } from 'ngx-bootstrap/chronos';
esLocale.invalidDate = "Formato no valido";
defineLocale('es', esLocale);

@NgModule({
  declarations: [
    ExpedientesComponent,
    FaseExpedienteComponent,
    ModalExpedientesInfoComponent,
    ModalTipoExpedientesComponent,
    DashboardComponent,
    ModalFiltrarFasesComponent,
    ModalFiltrosAvanzadosComponent,
    DashboardFasesComponent,
    DashboardFasesLateralComponent,
    TimelineComponent
  ],
  imports: [
    CommonModule,
    ReportesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    ComponentesHtmlModule,
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    TooltipModule.forRoot()
  ]
})
export class ReportesModule {
  constructor(private bsLocaleService: BsLocaleService) {
    this.bsLocaleService.use('es');
  }
}
