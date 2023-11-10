export interface ITotalExpedienteFaseModelo {
  idTipoExpediente: string;
  total: number;
  fases: IFaseExpedienteModelo[];
  icono: string;
  fechaInicial?: string;
  fechaFinal?: string;
  ejercicioFiscal?: number;
  titulo?: string;
}

export interface IEncabezadoRouteModelo {
  idTipoExpediente: string;
  fechaInicial?: string;
  fechaFinal?: string;
  ejercicioFiscal?: number;
  titulo?: string;
}

export interface IFaseExpedienteModelo {
    idTipoExpediente: string;
    idFase: string;
    descripcion: string;
    total: number;
    ultimoMes: number;
    path?: string;
    color?: string;
}

export interface IUsuarioExpedienteModelo {
  idTipoExpediente: string;
  idUsuario: string;
  idUnidadAdministrativa: string;
  extension: string;
  descripcion: string;
  total: number;
}

export interface IDatosDashboardModelo {
  idTipoExpediente: string;
  fechaInicial: Date;
  fechaFinal: Date;
  cantidadExpedientesPorFase: ITotalExpedienteFaseModelo;
  cantidadExpedientesPorUsuarios: IUsuarioExpedienteModelo[];
}
