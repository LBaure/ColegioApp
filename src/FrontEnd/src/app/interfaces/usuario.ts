

export interface UsuarioSSOModelo {
  nit: string;
  name: string;
  isAuthenticated: boolean;
  perfil: Perfil;
}

export interface Perfil {
  menu: Opcion[];
  estado: Estado;
  roles: Rol[];
}

export interface Estado {
  idEstado: number;
  nombre: string;
  estilo: string;
}

export interface Opcion {
  idOpcionMenu: number;
  IdOpcionMenuPadre: number;
  nombre: string;
  url: string;
  consulta: string;
  inserta: string;
  modifica: string;
  elimina: string;
  icono?: string;
  opciones: Opcion[];
}

export interface Rol {
  rolId: number;
  nombre: string;
}
export interface PermissionsModel {
  select: boolean;
  insert: boolean;
  update: boolean;
  delete: boolean;
}

export interface IPermisosUsuario {
  inserta: boolean;
  modifica: boolean;
  elimina: boolean;
  consulta: boolean;
}
