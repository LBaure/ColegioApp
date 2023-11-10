export interface IEncabezadoExpediente {
  expedienteGlobal: string;
  usuarioGrabo: string;
  idFaseActual: string;
  fechaGrabacion: string;
  idTipoExpediente: string;
  unidadAdministrativa: string;
  recibidoDe: string;
  origen: string;
  descripcionIngreso: string;
  observacionesIngreso: string;
}

export interface IDatosExpediente {
  etiqueta: string;
  valor: string;
}

export interface IHistoricoExpediente {
  descripcionExpediente: string;
  noExpediente: string;
  idFase: string;
  descripcionFase: string;
  fechaTraslado: string;
  idUsuario: string;
  usuario: string;
  observaciones: string;
}
