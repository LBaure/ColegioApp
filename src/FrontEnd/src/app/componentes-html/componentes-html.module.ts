import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentesHtmlRoutingModule } from './componentes-html-routing.module';
import { HomeComponent } from './home/home.component';
import { ButtonsComponent } from './components/buttons/buttons.component';
import { InputsComponent } from './components/inputs/inputs.component';
import { AlertsComponent } from './components/alerts/alerts.component';
import { FormsComponent } from './components/forms/forms.component';
import { TablesComponent } from './components/tables/tables.component';
import { HttpClientModule } from '@angular/common/http';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgxPaginationModule } from 'ngx-pagination';
import { PaginationComponent } from './components/pagination/pagination.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ModalsComponent } from './components/modals/modals.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { ViewFileComponent } from './components/view-file/view-file.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ListsComponent } from './components/lists/lists.component';
import { ToastrComponent } from './components/toastr/toastr.component';
import { LoadingMinfinComponent } from './components/loading-minfin/loading-minfin.component';
import { NgxLoadingModule } from "ngx-loading";
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { DropdownComponent } from './components/dropdown/dropdown.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

@NgModule({
  declarations: [
    HomeComponent,
    ButtonsComponent,
    InputsComponent,
    AlertsComponent,
    FormsComponent,
    TablesComponent,
    PaginationComponent,
    ModalsComponent,
    FileUploadComponent,
    ViewFileComponent,
    ListsComponent,
    ToastrComponent,
    LoadingMinfinComponent,
    DropdownComponent,
    BreadcrumbComponent
  ],
  imports: [
    CommonModule,
    ComponentesHtmlRoutingModule,
    HttpClientModule,
    NgxPaginationModule,
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
    ReactiveFormsModule,
    FormsModule,
    BsDatepickerModule.forRoot(),
    NgxLoadingModule.forRoot({}),
    BsDropdownModule.forRoot()
  ],
  exports: [
    LoadingMinfinComponent,
    BreadcrumbComponent
  ]
})
export class ComponentesHtmlModule { }
