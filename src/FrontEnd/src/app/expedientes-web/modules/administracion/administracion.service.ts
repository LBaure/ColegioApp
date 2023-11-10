import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { IRespuestaHttp } from '../../interfaces/compartido/resultado-http';

const URL : string = 'api/Catalogos'
@Injectable({
  providedIn: 'root'
})
export class AdministracionService {

  constructor(private http : HttpClient) { }


  // CATALOGOS
  catalogoOpcionesMenu() {
    const url = `${URL}/OpcionesMenu`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    );
  }

  catalogoRoles() {
    const url = `${URL}/Roles`;
    return this.http.get<IRespuestaHttp>(url).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let mensajeError : string = '';
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
    } else if (error.status === 400) {
      mensajeError = error.error.mensaje;
    } else if (error.status === 404) {
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
