export interface ITipoExpediente {
  idTipoExpediente: string;
  descripcion: string;
}

export interface IFasesExpediente {
  idFase: string;
  nombreFase: string;
  tipoFase: string;
  expedientes?: Expediente[]
}


export interface Expediente {
  expedienteGlobal: string;
  fechaGrabacion: string;
  tiempoTranscurrido: string;
  recibidoDe: string;
  origen: string;
  unidadAdministrativa: string;
  descripcionIngreso: string;
  usuarioAsignado: string;
}


export interface IExpedientesWorkFlow {
  noExpedienteMinfin: string;
  noExpediente: string;
  fechaGrabacion: string;
  tiempoTranscurrido: string;
  idTipoExpediente: string;
  descripcion: string;
  idFase: string;
  usuarioAsignado: string;
}

export interface IFasesConExpedientesWorkFlow {
  idFase: string
  descripcion: string
  idTipoExpediente: string
  expedientes: IExpedientesWorkFlow[]
}
