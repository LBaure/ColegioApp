import { Component, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IRol } from 'src/app/expedientes-web/interfaces/seguridad/rol';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../../seguridad.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ResizeService } from 'src/app/utils/resize.service';
import { IEstadosAdminstrativo } from 'src/app/expedientes-web/interfaces/estados-admin';
import { Notify } from 'notiflix';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-modal-roles',
  templateUrl: './modal-roles.component.html',
  styleUrls: ['./modal-roles.component.css']
})
export class ModalRolesComponent implements OnInit {
  private servicesPantalla$ : Subscription = new Subscription;
  mq? : SCREEN_SIZE;

  tituloModal: string = "";
  esEdicion : boolean = true;
  rol: IRol = {} as IRol;
  listadoEstados: IEstadosAdminstrativo[] = [];
  loading : boolean = false;
  formRoles: FormGroup = {} as FormGroup;
  public EventoGuardar = new EventEmitter<IRol[]>();

  constructor(
    private fb: FormBuilder,
    public bsModalRef: BsModalRef,
    private http: SeguridadService,
    private resizeService : ResizeService
  ) {
    this.formRoles = this.obtenerControlesFormulario();
  }

  private obtenerControlesFormulario(): FormGroup {
    const group: FormGroup = this.fb.group({
      idRol: [0],
      nombre: [null, Validators.required],
      descripcion: [null],
      activo: [true, Validators.required]
    })
    return group;
  }

  ngOnInit(): void {
    if(this.rol.idRol) {
      this.formRoles.patchValue(this.rol);
    }
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      this.mq = x;
      if (x === SCREEN_SIZE.XS || x === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    })

  }

  public guardarRegistro(): void {
    if (!this.formRoles.valid) {
      Object.values(this.formRoles.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }
    this.loading = true;
    const reqRol: IRol = {
      idRol: this.idRol?.value,
      nombre: this.nombre?.value,
      descripcion: this.descripcion?.value,
      activo: this.activo?.value
    }

    const metodoEjecutar = (this.esEdicion) ? 'modificarRol' : 'insertarRol';
    this.http[metodoEjecutar](reqRol)
      .subscribe({
        next: (resultado) => { this.administrarRespuesta(resultado); },
        error: (error) => {
          Notify.failure(error.message);
          this.loading = false;
        }
      })
  }

  private administrarRespuesta(resultado:IRespuestaHttp) : void {
    this.loading = false;
    if(resultado.estado === EstadosHttp.success) {
      this.bsModalRef.hide();
      this.EventoGuardar.emit(resultado.resultado)
    }

    if (this.mq === SCREEN_SIZE.XS || this.mq === SCREEN_SIZE.SM) {
      Notify[resultado.estado as keyof typeof Notify](resultado.mensaje, {
        position: 'center-center',
        fontSize: 'var(--bs-body-font-size)',
        clickToClose: true,
      })
    }
    else {
      Swal.fire({
        icon: resultado.icono,
        title: resultado.titulo,
        text: resultado.mensaje,
        showCloseButton: true
      });
    }
  }


  public get idRol() {return this.formRoles.get('idRol');}
  public get nombre() {return this.formRoles.get('nombre');}
  public get descripcion() {return this.formRoles.get('descripcion');}
  public get activo() {return this.formRoles.get('activo');}

}
