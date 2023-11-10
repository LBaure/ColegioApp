import { Injectable } from '@angular/core';
import { BehaviorSubject, Observer, catchError, throwError } from 'rxjs';
import { IRespuestaHttp } from '../../interfaces/compartido/resultado-http';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ITipoExpediente } from '../../interfaces/dashboard/tipos-expedientes';
import { IFiltrosEncabezado, IFiltrosExpedientesPorTipo } from '../../interfaces/dashboard/filtros-expedientes-por-tipo';
import { IFiltrosAvanzadosPorTipoExpediente } from '../../interfaces/dashboard/filtros-avanzados-por-tipo-expediente';

const URL : string = 'api/dashboard/FasesExpediente'
const URLdashboard : string = 'api/dashboard/Dashboard'

export interface FaseInfo {
  id: number;
  titulo: string;
  expedientes?: Expediente[]
}

export interface Expediente {
  id: number;
  titulo: string;
  descripcion: string;
  usuario?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReportesService {

  constructor(private http : HttpClient) { }

  ObtenerTipoExpedientes() {
    const url = `${URL}`;
    return this.http.get<ITipoExpediente[]>(url).pipe(
      catchError(this.handleError)
    );
  }

  obtenerExpedientesWorkFlow(filtros: IFiltrosAvanzadosPorTipoExpediente) {
    const url = `${URL}/ObtenerExpedientesWorkFlow/`;
    return this.http.post<IRespuestaHttp>(url, filtros).pipe(
      catchError(this.handleError)
    );
  }

  obtenerDatosDashboard(filtros: IFiltrosExpedientesPorTipo) {
    const url = `${URLdashboard}/ObtenerDatosDashboard`;
    return this.http.post<IRespuestaHttp>(url, filtros).pipe(
      catchError(this.handleError)
    );
  }

  obtenerExpedientesPorTipo(filtros: IFiltrosAvanzadosPorTipoExpediente) {
    const url = `${URLdashboard}/ExpedientesPorTipo`;
    return this.http.post<IRespuestaHttp>(url, filtros)
      .pipe(
        catchError(this.handleError)
      );
  }

  obtenerEncabezadoExpediente(filtros: IFiltrosEncabezado) {
    const url = `${URLdashboard}/EncabezadoExpediente`;
    return this.http.post<IRespuestaHttp>(url, filtros)
      .pipe(
        catchError(this.handleError)
      );
  }

  obtenerFasesPorTipoExpediente(idTipoExpediente: string) {
    const url = `${URLdashboard}/ObtenerFasesPorTipoExpediente/${idTipoExpediente}`;
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
