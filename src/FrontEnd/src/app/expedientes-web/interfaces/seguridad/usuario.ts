export interface IUsuario {
  nitUsuario: string;
  nombreCompleto: string;
  emailInstitucional: string;
  emailPersonal: string;
  cargo: string;
  telefono: string;
  fotoPerfil: string;
  fotoFisica: string;
  fechaRegistro: string;
  activo: boolean;
  dobleFactorAuth: boolean;
  usuarioGuardo: string;
  intentosLogin: number;
  dobleFactorObligatorio: boolean;
  existePolitica: boolean
}

export interface IUsuarioAdministracion {
  nitUsuario: string;
  nombreCompleto : string;
  emailInstitucional : string;
  cargo? : string;
  activo : boolean;
  dobleFactorObligatorio: boolean;
}

export interface IUsuarioRoles {
  nitUsuario : string;
  roles: string;
}


export interface IBitacoraUsuarioRoles {
  nitUsuario : string;
  rol : string;
  usuarioRegistro : string;
  nombreUsuarioRegistro: string;
  fechaRegistro : string;
  activo : string;
}


export interface IMiUsuario {
  nitUsuario: string;
  nombreCompleto: string;
  emailPersonal: string;
  telefono: string;
}


export interface ICambioPinContrasenia {
  passwordCurrent: string;
  newPassword: string;
  confirmNewPassword: string;
}
