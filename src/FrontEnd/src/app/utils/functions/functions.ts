import { Injectable } from "@angular/core";


@Injectable({
  providedIn: 'root'
})

export class Functions {
  constructor () {}


  /**
   * Filtrar registros de la tabla
   * @param property
   * @param sort_order
   * @returns
   */
  public createCompareFn<T extends Object> (property: keyof T, sort_order: "asc" | "desc") {
    const compareFn = (a: T, b: T) => {
      const val1 = a[property];
      const val2 = b[property];
      const order = sort_order !== "desc" ? 1 : -1;

      switch (typeof val1) {
        case "number": {
          const valb = val2  as unknown as number;
          const result = val1 - valb;
          return result * order;
        }
        case "string": {
          const valb = val2 as unknown as string;
          const result = val1.localeCompare(valb);
          return result * order;
        }
        // add other cases like boolean, etc.
        default:
          return 0;
      }
    };
    return compareFn;
  }

  public transformarFecha(fecha:string): string {
    let fechaFormateada:string = '';
    let listaFecha = fecha.split("/")
    if (listaFecha.length === 3) {
      fechaFormateada = `${listaFecha[1]}/${listaFecha[0]}/${listaFecha[2]}`
    }
    return fechaFormateada;
  }



  soloCaracteres(evt: KeyboardEvent) {
    var keyCode = (evt.which) ? evt.which : evt.keyCode
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) || keyCode === 32) {
      evt.preventDefault();
    }
  }

  soloNumerosEnteros(evt: KeyboardEvent){
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57) || charCode === 46) {
      evt.preventDefault();
    }
  }



}
