import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlertsComponent } from './components/alerts/alerts.component';
import { ButtonsComponent } from './components/buttons/buttons.component';
import { DropdownComponent } from './components/dropdown/dropdown.component';
import { FormsComponent } from './components/forms/forms.component';
import { InputsComponent } from './components/inputs/inputs.component';
import { ListsComponent } from './components/lists/lists.component';
import { ModalsComponent } from './components/modals/modals.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { TablesComponent } from './components/tables/tables.component';
import { ToastrComponent } from './components/toastr/toastr.component';

const routes: Routes = [
  {
    path: 'buttons',
    component: ButtonsComponent,
  },
  {
    path: 'inputs',
    component: InputsComponent,
  },
  {
    path: 'alerts',
    component: AlertsComponent,
  },
  {
    path: 'forms',
    component: FormsComponent,
  },
  {
    path: 'tables',
    component: TablesComponent,
  },
  {
    path: 'pagination',
    component: PaginationComponent,
  },
  {
    path: 'modals',
    component: ModalsComponent
  },
  {
    path: 'lists',
    component: ListsComponent
  },
  {
    path: 'toastr',
    component: ToastrComponent
  },
  {
    path: 'dropdown',
    component: DropdownComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ComponentesHtmlRoutingModule {}
