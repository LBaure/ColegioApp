import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsuariosComponent } from './usuarios/usuarios.component';
import { AuthGuardService } from 'src/app/utils/auth-guard.service';
import { OpcionesMenuComponent } from './opciones-menu/opciones-menu.component';
import { RolesComponent } from './roles/roles.component';
import { RolesOpcionesMenuComponent } from './roles-opciones-menu/roles-opciones-menu.component';
import { MiPerfilComponent } from './mi-perfil/mi-perfil.component';

const routes: Routes = [
  {
    path: 'usuarios',
    component: UsuariosComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'opciones-menu',
    component: OpcionesMenuComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'roles',
    children: [
      {
        path: '',
        component: RolesComponent,
        canActivate: [AuthGuardService],
      },
      {
        path: 'opciones-menu',
        component: RolesOpcionesMenuComponent
      }
    ]
  },
  {
    path: 'mi-perfil',
    component: MiPerfilComponent,
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SeguridadRoutingModule { }
