import { Component, Input, SimpleChanges } from '@angular/core';
import { ECDH } from 'crypto';
import { IEncabezadoRouteModelo, IFaseExpedienteModelo, ITotalExpedienteFaseModelo } from 'src/app/expedientes-web/interfaces/dashboard/total-expediente-fase';

@Component({
  selector: 'app-dashboard-fases-lateral',
  templateUrl: './dashboard-fases-lateral.component.html',
  styleUrls: ['./dashboard-fases-lateral.component.css']
})
export class DashboardFasesLateralComponent {

  @Input() items: ITotalExpedienteFaseModelo = {} as ITotalExpedienteFaseModelo;
  itemsFases: ITotalExpedienteFaseModelo = {} as ITotalExpedienteFaseModelo;
  encabezado: IEncabezadoRouteModelo = {} as IEncabezadoRouteModelo;

  ngOnInit(): void {

    if (JSON.stringify(this.items) === '{}') {
      //console.log('PRUEBA LATERAL',JSON.stringify(this.items));
      return;

    }else{
      this.encabezado.ejercicioFiscal = this.items.ejercicioFiscal;
      this.encabezado.fechaFinal = this.items.fechaFinal;
      this.encabezado.fechaInicial = this.items.fechaInicial;
      this.encabezado.idTipoExpediente = this.items.idTipoExpediente;
      this.encabezado.titulo = this.items.titulo;
    }
  }
  ngOnChanges(changes: SimpleChanges): void {

    if (JSON.stringify(this.items) === '{}') {
      //console.log(JSON.stringify(this.items));
      return;

    }else{
      this.encabezado.ejercicioFiscal = this.items.ejercicioFiscal;
      this.encabezado.fechaFinal = this.items.fechaFinal;
      this.encabezado.fechaInicial = this.items.fechaInicial;
      this.encabezado.idTipoExpediente = this.items.idTipoExpediente;
      this.encabezado.titulo = this.items.titulo;
    }



    if (!this.items) return;
    this.itemsFases = Object.assign({}, this.items)
  }


  obtenerEstilos(color?: string) {
    let colorFuente = `color: ${color};`
    let background = color?.replace(')', '');
    background = `background: ${background}, 40%);`
    return background + colorFuente;
  }
}
