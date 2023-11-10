import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SeguridadRoutingModule } from './seguridad-routing.module';
import { UsuariosComponent } from './usuarios/usuarios.component';
import { ModalAgregarRolesUsuarioComponent } from './componentes/modal-agregar-roles-usuario/modal-agregar-roles-usuario.component';
import { MiInformacionPersonalComponent } from './componentes/mi-informacion-personal/mi-informacion-personal.component';

import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalUsuarioAdminComponent } from './componentes/modal-usuario-admin/modal-usuario-admin.component';
import { OpcionesMenuComponent } from './opciones-menu/opciones-menu.component';
import { ModalOpcionesMenuComponent } from './componentes/modal-opciones-menu/modal-opciones-menu.component';
import { RolesComponent } from './roles/roles.component';
import { ModalRolesComponent } from './componentes/modal-roles/modal-roles.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RolesOpcionesMenuComponent } from './roles-opciones-menu/roles-opciones-menu.component';
import { ComponentesHtmlModule } from 'src/app/componentes-html/componentes-html.module';
import { ModalRolesOpcionMenuComponent } from './componentes/modal-roles-opcion-menu/modal-roles-opcion-menu.component';
import { BitacoraRolesUsuarioComponent } from './componentes/bitacora-roles-usuario/bitacora-roles-usuario.component';
import { MiPerfilComponent } from './mi-perfil/mi-perfil.component';
import { PerfilComponent } from './componentes/perfil/perfil.component';
import { PoliticaPrivacidadComponent } from './componentes/politica-privacidad/politica-privacidad.component';
import { CambiarSeguridadComponent } from './componentes/cambiar-seguridad/cambiar-seguridad.component';
import { TabsModule } from 'ngx-bootstrap/tabs';

@NgModule({
  declarations: [
    UsuariosComponent,
    ModalAgregarRolesUsuarioComponent,
    MiInformacionPersonalComponent,
    ModalUsuarioAdminComponent,
    OpcionesMenuComponent,
    ModalOpcionesMenuComponent,
    RolesComponent,
    ModalRolesComponent,
    RolesOpcionesMenuComponent,
    ModalRolesOpcionMenuComponent,
    BitacoraRolesUsuarioComponent,
    MiPerfilComponent,
    PerfilComponent,
    PoliticaPrivacidadComponent,
    CambiarSeguridadComponent
  ],
  imports: [
    CommonModule,
    ComponentesHtmlModule,
    SeguridadRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot()
  ]
})
export class SeguridadModule { }
