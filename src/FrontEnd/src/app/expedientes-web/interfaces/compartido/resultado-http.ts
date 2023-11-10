import { EstadosHttp } from "../../constants/estado-http";
export interface IRespuestaHttp {
  estado: EstadosHttp;
  mensaje: string;
  resultado: any;
  icono: iconoSwal;
  titulo: string;
}

export enum iconoSwal {
  success = 'success',
  warning = 'warning',
  info = 'info',
  error = 'error',
  question = 'question'
}
