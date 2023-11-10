export interface ICatalogos {
  idCatalogo : number;
  nombre : string;
  descripcion? : string;
  tipoCatalogo? : number;
  activo? : number;
  tipoCatalogoTexto? : string;
}


export interface IBreadcrumb {
  title: string;
  path: string;
  disabled?: boolean;
  state?: any;
}
