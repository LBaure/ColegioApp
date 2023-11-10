import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { IMiUsuario, IUsuario, IUsuarioAdministracion, IUsuarioRoles } from '../../interfaces/seguridad/usuario';
import { IRespuestaHttp } from '../../interfaces/compartido/resultado-http';
import { IListaOpcionMenu, IOpcionMenu } from '../../interfaces/seguridad/opciones-menu';
import { IRol, IRolOpcionMenu, IRolOpcionMenuDetalle } from '../../interfaces/seguridad/rol';
import { IEstadosAdminstrativo } from '../../interfaces/estados-admin';
import { ICambioCredenciales, IPoliticaPrivacidad, IPoliticaUsuario } from '../../interfaces/seguridad/politica-privacidad';
import { ICredencialesUsuario } from '../../interfaces/Login';

const URL : string = 'api/Seguridad'
@Injectable({
  providedIn: 'root'
})

export class SeguridadService {
  configUrl = 'assets/files/estados.json';

  constructor(private http : HttpClient) { }

  getConfig() {
    return this.http.get<IEstadosAdminstrativo[]>(this.configUrl);
  }

  login(data: ICredencialesUsuario) {
    const url = 'api/seguridad/usuarios/login';
    return this.http.post<IRespuestaHttp>(url, data).pipe(
      catchError(this.handleError)
    )
  }


  obtenerUsuarios(nitUsuario?:string){
    let url = `${URL}/Usuarios`;
    url += nitUsuario ? `/${nitUsuario}` : '';
    return this.http.get<IUsuario[]>(url)
    .pipe(
      catchError(this.handleError)
    );
  }

  modificarUsuario(usuario:IUsuarioAdministracion) {
    const url = `${URL}/Usuarios`;
    return this.http.put<IRespuestaHttp>(url, usuario)
      .pipe(
        catchError(this.handleError)
      );
  }
  insertarUsuario(usuario:IUsuarioAdministracion) {
    const url = `${URL}/Usuarios`;
    return this.http.post<IRespuestaHttp>(url, usuario)
      .pipe(
        catchError(this.handleError)
      );
  }

  eliminarUsuario(usuario: IUsuarioAdministracion) {
    const url = `${URL}/Usuarios`;
    const options = { body: usuario }
    return this.http.delete<IRespuestaHttp>(url, options)
      .pipe(
        catchError(this.handleError)
      );
  }

  obtenerRolesUsuario(nitUsuario: string) {
    const url = `${URL}/Usuarios/Roles/${nitUsuario}`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    );
  }

  insertarRolesUsuario(usuario : IUsuarioRoles) : Observable<any> {
    const url = `${URL}/Usuarios/Roles`;
    return this.http.post<IRespuestaHttp>(url, usuario).pipe(
      catchError(this.handleError)
    )
  }

  obtenerBitacoraRolesUsuario(nitUsuario: string) {
    const url = `${URL}/Usuarios/BitacoraRoles/${nitUsuario}`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    );
  }


  // OPCIONES DEL MENU
  obtenerOpcionesMenu() {
    const url = `${URL}/opcionesMenu`;
    return this.http.get<IListaOpcionMenu[]>(url).pipe(
      catchError(this.handleError)
    );
  }

  insertarRegistroOpcionMenu(opcionMenu : IOpcionMenu): Observable<IRespuestaHttp>{
    const url = `${URL}/opcionesMenu`;
    return this.http.post<IRespuestaHttp>(url, opcionMenu).pipe(
      catchError(this.handleError)
    );
  }

  actualizarRegistroOpcionMenu(opcionMenu : IOpcionMenu): Observable<IRespuestaHttp>{
    const url = `${URL}/opcionesMenu`;
    return this.http.put<IRespuestaHttp>(url, opcionMenu).pipe(
      catchError(this.handleError)
    );
  }

  eliminarRegistroOpcionMenu(data : IOpcionMenu) : Observable<any> {
    const url = `${URL}/opcionesMenu/${data.idOpcionMenu}`;
    return this.http.delete<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  // ROLES
  obtenerRoles() : Observable<any> {
    const url = `${URL}/roles`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  insertarRol(rol : IRol) : Observable<any> {
    const url = `${URL}/roles`;
    return this.http.post<IRespuestaHttp>(url, rol).pipe(
      catchError(this.handleError)
    )
  }

  modificarRol(rol : IRol) : Observable<any> {
    const url = `${URL}/roles`;
    return this.http.put<IRespuestaHttp>(url, rol).pipe(
      catchError(this.handleError)
    )
  }

  eliminarRol(rol:IRol) : Observable<any> {
    const url = `${URL}/roles/${rol.idRol}`;
    return this.http.delete<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  // OPCIONES MENU - POR ROL
  obtenerOpcionesMenuPorRol(rol : IRol) {
    const url = `${URL}/roles/opciones-menu/${rol.idRol}`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  insertarOpcionMenuPorRol(datos : IRolOpcionMenu)  : Observable<any> {
    const url = `${URL}/roles/opciones-menu`;
    return this.http.post<IRespuestaHttp>(url, datos).pipe(
      catchError(this.handleError)
    )
  }

  modificarOpcionMenuPorRol(datos : IRolOpcionMenu)  : Observable<any> {
    const url = `${URL}/roles/opciones-menu`;
    return this.http.put<IRespuestaHttp>(url, datos).pipe(
      catchError(this.handleError)
    )
  }

  eliminarRolOpcionMenu(opcionMenu : IRolOpcionMenuDetalle) : Observable<any> {
    const url = `${URL}/roles/opciones-menu`;
    const options = {
      body: opcionMenu
    }
    return this.http.delete<IRespuestaHttp>(url, options).pipe(
      catchError(this.handleError)
    )
  }

  actualizarMiPerfil(data : IMiUsuario) : Observable<any> {
    const url = `${URL}/Usuarios/MiPerfil`;
    return this.http.put<IRespuestaHttp>(url, data).pipe(
      catchError(this.handleError)
    )
  }


  obtenerPoliticasPrivacidad() {
    const url = `${URL}/PoliticaPrivacidad`;
    return this.http.get<IPoliticaPrivacidad[]>(url).pipe(
      catchError(this.handleError)
    )
  }

  obtenerConfiguracionDobleFactor() {
    const url = `${URL}/PoliticaPrivacidad/ConfiguracionDobleFactor`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  obtenerConfiguracionDobleFactorUsuario() {
    const url = `${URL}/PoliticaPrivacidad/ConfiguracionDobleFactorLogueado`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }



  insertarPoliticaUsuario(datos : IPoliticaUsuario)  : Observable<any> {
    const url = `${URL}/PoliticaPrivacidad`;
    return this.http.post<IRespuestaHttp>(url, datos).pipe(
      catchError(this.handleError)
    )
  }

  eliminarDobleFactor(valor:string) : Observable<any> {
    const url = `${URL}/PoliticaPrivacidad/${valor}`;
    return this.http.delete<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    )
  }

  actualizarPoliticaUsuario(datos: ICambioCredenciales) : Observable<any> {
    const url = `${URL}/PoliticaPrivacidad/ActualizarPolitica`;
    return this.http.post<IRespuestaHttp>(url, datos).pipe(
      catchError(this.handleError)
    )
  }


  private handleError(error: HttpErrorResponse) {
    let mensajeError : string = '';
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
    } else if (error.status === 400) {
      mensajeError = error.error.mensaje;
    } else if (error.status === 401) {
      mensajeError = error.error.mensaje;
    }else if (error.status === 404) {
      mensajeError = 'Lo sentimos, no hemos podido encontrar el recurso al que intentas acceder.'
    } else if (error.status === 405) {
      mensajeError = 'Lo sentimos, no hemos podido reconocer el metodo del recurso al que intentas acceder.'
    } else if (error.status === 504) {
      mensajeError = 'Se ha agotado el tiempo de espera, no se ha podido establecer comunicacion el servidor.'
    } else {
      // El backend devolvió un código de respuesta inútil.
      // El cuerpo de la respuesta puede contener pistas sobre lo que salió mal.
      mensajeError = `Backend returned code ${error.status}, body was: `, error.error;
    }
    return throwError(() => new Error(mensajeError));
  }

}
