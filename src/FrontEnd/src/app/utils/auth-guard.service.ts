import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { localStorageCore } from './functions/localStorageCore';
import { Opcion } from '../interfaces/usuario';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService {
  dobleFactor : string = '';
  ruta : string = "";
  userPaths: Opcion[] = [ {
    idOpcionMenu: 0,
    IdOpcionMenuPadre: 0,
    nombre: '',
    url: '/ew/dashboard',
    opciones:[],
    consulta: 'S',
    inserta: 'S',
    modifica: 'S',
    elimina: 'S',
  }];

  constructor(
    private auth: AuthService,
    private router: Router,
    private local: localStorageCore
  ) {
    this.dobleFactor = this.local.getItem("logToken");
  }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.auth.validarUsuarioAutenticado()) {
      if (state.url === '/ew/seguridad/mi-perfil') {
        this.local.setItem("lastRoute", state.url);
        return true;
      } else if( this.dobleFactor) {
        return this.obtenerPermisos(state);
      } else {
        this.router.navigate(['not-found']);
        return false;
      }
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }

  obtenerPermisos(state: RouterStateSnapshot) : boolean {
    const currentRoute = state.url;
    const menusAllows = this.auth.obtenerMenuUsuario();
    this.unirTodasRutasUsuario(menusAllows);
    var filteredPath = this.userPaths.find(element => element.url === currentRoute);
    if (!filteredPath) {
      this.router.navigate(['not-found']);
      return false
    } else {
      this.local.setItem("currentRoute", JSON.stringify(filteredPath));
      this.local.setItem("lastRoute", state.url);
      return true;
    }
  }

  private unirTodasRutasUsuario(opciones : Opcion[]) {
    opciones.forEach(element => {
      if (element.opciones.length>0) {
        this.unirTodasRutasUsuario(element.opciones)
      } else{
        this.userPaths.push(element)
      }
    })
  }
}
