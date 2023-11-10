import { TiposEstado } from "../constants/Estados";

export interface ICredencialesUsuario {
  nitUsuario : string;
  valor? : string;
  dobleFactor? : boolean;
}
