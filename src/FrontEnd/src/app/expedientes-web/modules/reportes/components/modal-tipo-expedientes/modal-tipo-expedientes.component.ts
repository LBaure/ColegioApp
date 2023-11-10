import { Component, EventEmitter, OnInit } from '@angular/core';
import { ReportesService } from '../../reportes.service';
import { Notify } from 'notiflix';
import { ITipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ResizeService } from 'src/app/utils/resize.service';
import { Subscription } from 'rxjs';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
@Component({
  selector: 'app-modal-tipo-expedientes',
  templateUrl: './modal-tipo-expedientes.component.html',
  styleUrls: ['./modal-tipo-expedientes.component.css']
})
export class ModalTipoExpedientesComponent implements OnInit {
  public EnviarTipoExpediente = new EventEmitter<ITipoExpediente>();
  servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;

  public close: boolean = false;
  listaTiposExpedientes: ITipoExpediente[] = [];

  /**
   *
   */
  constructor(
    private http: ReportesService,
    private resizeService : ResizeService,
    public bsModalRef: BsModalRef
  ) { }
  ngOnInit(): void {
    this.controlTamanioPantalla();

    this.http.ObtenerTipoExpedientes().subscribe({
      next: (data) => {
        this.listaTiposExpedientes = data;
      },
      error : (error) => { Notify.failure(error.message); }
    })
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


  setDatos(tipoExpediente: ITipoExpediente) {
    this.EnviarTipoExpediente.emit(tipoExpediente);
  }
}
