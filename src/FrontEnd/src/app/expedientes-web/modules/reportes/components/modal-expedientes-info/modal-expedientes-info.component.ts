import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ReportesService } from '../../reportes.service';
import { IFiltrosEncabezado } from 'src/app/expedientes-web/interfaces/dashboard/filtros-expedientes-por-tipo';
import { Notify } from 'notiflix';
import { IDatosExpediente, IEncabezadoExpediente, IHistoricoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/encabezado-expediente';
import { Subscription } from 'rxjs';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { ResizeService } from 'src/app/utils/resize.service';

@Component({
  selector: 'app-modal-expedientes-info',
  templateUrl: './modal-expedientes-info.component.html',
  styleUrls: ['./modal-expedientes-info.component.css']
})
export class ModalExpedientesInfoComponent {

  servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;

  titulo?: string;
  closeBtnName?: string;
  list: any = {};

  encabezado: IEncabezadoExpediente = {} as IEncabezadoExpediente;
  listaDatos: IDatosExpediente[] = [];
  listaHistorico: IHistoricoExpediente[] = [];

  constructor(
    private resizeService : ResizeService,
    public bsModalRef: BsModalRef,
    private http : ReportesService

  ) {}

  ngOnInit() {
    //console.log("list", this.list);

    this.controlTamanioPantalla();
    this.obtenerEncabezadoExpediente();
  }

  controlTamanioPantalla() {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      this.mq = x;
      if (x === SCREEN_SIZE.XS || x === SCREEN_SIZE.SM || x === SCREEN_SIZE.MD) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-xl modal-dialog-centered')
      }
    });
  }

  obtenerEncabezadoExpediente() {
    let filtros: IFiltrosEncabezado  = {
      expedienteGlobal: this.list.expedienteGlobal,
      idTipoExpediente: this.list.idTipoExpediente,
      noExpediente: this.list.noExpediente,
    }

    this.http.obtenerEncabezadoExpediente(filtros).subscribe({
      next: (data) => {
        //console.log("obtenerEncabezadoExpediente", data);
        if (data.resultado) {
          this.encabezado = data.resultado.encabezado;
          this.listaDatos = data.resultado.datos;
          this.listaHistorico = data.resultado.historico;
        }

      },
      error : (error) => { Notify.failure(error.message); }

    })
  }
}
