export interface IRol {
  idRol? : number;
  nombre : string;
  descripcion : string;
  activo : boolean;
  contadorOpcionesMenu?: number;
}

export interface IRolOpcionMenu {
  idRol : number;
  idOpcionMenu : number;
  consulta : string;
  inserta : string;
  modifica : string;
  elimina : string;
}

export interface IRolOpcionMenuDetalle extends IRolOpcionMenu {
  rol?: string;
  opcionMenu?: string;
  descripcionOpcion?: string;
}



