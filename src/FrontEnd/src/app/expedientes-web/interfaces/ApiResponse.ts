export interface ApiResponse {
  estado: string;
  icono: icon;
  mensaje: string;
  titulo: string;
  data?: any;
}

export interface IResponseError {
  message: string;
  status: string;
  icon: string;
}


export enum icon {
  success = 'success',
  warning = 'warning',
  info = 'info',
  error = 'error',
  question = 'question'
}


export enum ResponseStatus {
  success = 'success',
  warning = 'warning',
  info = 'info',
  error = 'error',
  question = 'question'
}
