export interface ICamposHtmlssin {
  etiqueta: string;
  pattern: string;
  required: boolean;
  messagePattern : string;
  messageMinLength : string;
  messageRequired : string;
  maxLength: number;
  minLength: number;
  mask?: string;
  class?: string;
  descripcion?: string;
  descriptionLogin?: string;
}
