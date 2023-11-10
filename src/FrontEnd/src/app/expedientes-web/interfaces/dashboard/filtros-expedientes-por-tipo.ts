export interface IFiltrosExpedientesPorTipo {
  idTipoExpediente?: string;
  fechaInicial?: string;
  fechaFinal?: string;
  ejercicioFiscal?: number;
}


export interface IFiltrosEncabezado {
  noExpediente?: number;
  expedienteGlobal?: string;
  idTipoExpediente?: string;
}
