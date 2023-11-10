export interface IPoliticaPrivacidad {
  idPolitica : number;
  nombre: string;
  descripcion : string;
  activo : boolean;
  etiqueta: string;
  minlength: number;
  pattern: string;
  mensajePattern: string;
  mensajeRequired: string;
  mensajeMinlength: string;
  maxlength: number;
  mascara: string;
  clase: string;
}

export interface ICamposHtml extends IPoliticaPrivacidad {
  required: boolean
}


export interface IPoliticaPrivacidadUsuario {
  nitUsuario: string;
  dobleFactorAuth: number;
  intentosLogin: number;
  idPolitica: number;
  valor: string;
  valorAnterior?: string;
}


export interface IPoliticaUsuario {
  nitUsuario: string;
  intentosLogin: number;
  idPolitica: number;
  valor: string;
}

export interface ICambioCredenciales {
  valorActual?: string,
  idPolitica: number | null;
  valorNuevo?: string,
  intentosLogin: number
}
