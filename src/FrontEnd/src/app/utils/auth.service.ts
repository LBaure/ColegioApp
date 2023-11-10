import { HostListener, Injectable } from '@angular/core';
import { localStorageCore } from './functions/localStorageCore';
import { Router } from '@angular/router';
import { StartupConfigurationService } from './startup-configuration.service';
import { Subject } from 'rxjs';
import { IPermisosUsuario } from '../interfaces/usuario';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  InactividadUsuario: Subject<any> = new Subject();
  usuarioActivo: any;
  esAutenticado : boolean = false;

  constructor(
    private localStorage: localStorageCore,
    private router: Router,
    private config : StartupConfigurationService
  ) {
    this.setTimeout();
    this.InactividadUsuario.subscribe(() => this.logout() ) ;
    this.esAutenticado = this.validarUsuarioAutenticado();
  }

  public validarUsuarioAutenticado() : boolean {
    let activo : boolean = false;
    if (this.config) {
      if (this.config.usuario) {
        if (this.config.usuario.isAuthenticated) {
          activo = true;
        }
      }
    }
    return activo;
  }

  public isAuthenticated() : boolean {
    if (!this.config) return false;
    if (!this.config.usuario) return false;
    return this.config.usuario.isAuthenticated;
  }

  public navigate(path:string) {
    if (path == undefined && path == null) return;
    this.router.navigate([path])
  }


  public obtenerUsuario () {
    if (!this.validarUsuarioAutenticado) {
      this.router.navigate(['login']);
    }
    return this.config.usuario;
  }

  public obtenerMenuUsuario() {
    if (!this.validarUsuarioAutenticado) {
      this.router.navigate(['login']);
    }
    return this.config.usuario.perfil.menu;
  }

  obtenerToken():string {
    return this.localStorage.getItem("logToken")
  }

  public obtenerPermisos() : IPermisosUsuario {
    var rutaAlmacenada = JSON.parse(this.localStorage.getItem("currentRoute"));
    let permsisos : IPermisosUsuario = {
      consulta: rutaAlmacenada.consulta,
      elimina: rutaAlmacenada.elimina,
      inserta: rutaAlmacenada.inserta,
      modifica: rutaAlmacenada.modifica
    }
    return permsisos;
  }

  setTimeout() {
    this.usuarioActivo = setTimeout(() => this.InactividadUsuario.next(undefined),  60 * 60 * 1000); // 5 minutos de inactividad y se desconecta la sesion
  }

  @HostListener('window:mousemove') refreshUserState() {
    clearTimeout(this.usuarioActivo);
    this.setTimeout();
  }

  logout() {
    this.config.logout();
  }

}
