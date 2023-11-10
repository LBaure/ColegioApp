export interface IRoles {
  idRol? : number;
  nombre : string;
  descripcion : string;
  activo : number;
}


export interface IOpcionesMenu {
  idOpcionMenu? : number;
  idOpcionMenuPadre? : number;
  nombre : string;
  menuPadre? : string;
  descripcion? : string;
  icono? : string;
  url? : string;
  orden : number;
  activo? : number;
  opciones? : IOpcionesMenu[];
}


export interface Opcion {
  idOpcionMenu?: number;
  idOpcionPadre?: number;
  nombre: string;
  url?: string;
  opciones?: Opcion[];
  orden : number;
  icon? : string;
}



export interface IRolesOpcionMenu {
  idRol : number;
  idOpcionMenu : number;
  rol?: string;
  opcionMenu?: string;
  descripcionOpcion?: string;
  consulta : string;
  inserta : string;
  modifica : string;
  elimina : string;
}
