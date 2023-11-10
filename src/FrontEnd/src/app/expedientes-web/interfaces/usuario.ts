import { TiposEstado } from "../constants/Estados";

export interface IUsuario {
  nitUsuario : string;
  nombreCompleto : string;
  emailInstitucional : string;
  emailPersonal : string;
  cargo : string;
  telefono : string;
  fotoPerfil : string;
  fotoFisica : string;
  fechaRegistro : string;
  dobleFactorAuth: number;
  intentosLogin: number;
  activo : number;
  dobleFactorObligatorio?: number;
  existePolitica?: number
}

export interface INewUsuario {
  nitUsuario: string;
  nombreCompleto : string;
  emailInstitucional : string;
  cargo? : string;
  activo : number;
  dobleFactorAuth: number;
  UsuarioLogin: string;
}


export interface IUsuarioRoles {
  nitUsuario : string;
  roles: string;
  nitUsuarioLogin: string;
}

export interface MyUserModel {
  nitUsuario: string;
  nombreCompleto : string;
  emailPersonal : string;
  telefono : string;
}

export interface PolicySecurityModel {
  idPolitica : number;
  nombre: string;
  descripcion : string;
  etiqueta: string;
  pattern: string;
  minlength: number;
  messagePattern: string;
  messageRequired: string;
  messageMinlength: string;
  activo : number;
}

export interface PrivacyPolicyUserModel {
  nitUsuario: string;
  dobleFactorAuth: number;
  intentosLogin: number;
  idPolitica: number;
  valor: string;
  valorAnterior?: string;
}

export interface UserPinPassModel {
  nitUsuarioLogin: string;
  passwordCurrent: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface ConfigUserModel {
  nitUsuario : string;
  dobleFactorAuth : boolean;
  numberentosLogin : number;
  idPolitica : number;
  descripcion : string;
  activo : boolean;
  etiqueta : string;
  minLength : number;
  pattern : string;
  messagePattern : string;
  messageRequired : string;
  messageMinlength : string;
  maxLength : number;
  mask : string;
  class : string;
  sesionActiva: boolean;
  descriptionLogin: string;
}

export interface userCredentialsModel {
  nitUsuario : string;
  valor?: string;
  dobleFactorAuth?: number;
}


export interface IReqMiPerfil {
  token: string;
  nitUsuario? : string;
}
