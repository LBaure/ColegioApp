import { Component, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IUsuario, IUsuarioAdministracion } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../../seguridad.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ResizeService } from 'src/app/utils/resize.service';
import Swal from 'sweetalert2';
import { Notify } from 'notiflix';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';

@Component({
  selector: 'app-modal-usuario-admin',
  templateUrl: './modal-usuario-admin.component.html',
  styleUrls: ['./modal-usuario-admin.component.css']
})
export class ModalUsuarioAdminComponent implements OnInit {
  tituloModal: string = "";
  esEdicion: boolean = false;
  usuarioModelo?: IUsuario;
  eventoGuardarUsuario = new EventEmitter<IUsuario>();
  bloquearBotones: boolean = false;
  FormUsuario : FormGroup = {} as FormGroup;
  servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;
  itemsEstados : any = [
    {
      text: "Activo", value: true
    },
    {
      text: "Inactivo", value: false
    }
  ]



  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private resizeService : ResizeService,
    private http: SeguridadService
  ) { this.FormUsuario = this.obtenerControlesFormulario(); }

  ngOnInit(): void {
    this.controlTamanioPantalla();
    if (this.esEdicion) {
      this.FormUsuario.patchValue(this.usuarioModelo ? this.usuarioModelo : {});
      this.dobleFactorObligatorio?.setValue(this.usuarioModelo?.dobleFactorObligatorio ? true : false)
      this.nitUsuario?.disable()
    }
  }

  controlTamanioPantalla() {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      this.mq = x;
      if (x === SCREEN_SIZE.XS || x === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    });
  }

  obtenerControlesFormulario(): FormGroup  {
    const group: FormGroup = this.fb.group({
      nitUsuario: [null, Validators.required],
      nombreCompleto: [null, Validators.required],
      emailInstitucional: [null, [Validators.required, Validators.pattern('^[a-z]+[a-z0-9._-]+@[a-z]+\.[a-z.]{2,8}$')]],
      cargo: [null],
      dobleFactorObligatorio: [true, Validators.required],
      activo: [true, Validators.required]
    })
    return group;
  }

  public guardarRegistro() : void {
    if (!this.FormUsuario.valid) {
      Object.values(this.FormUsuario.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }
    this.bloquearBotones = true;

    const usuarioModelo: IUsuarioAdministracion = {
      nitUsuario: this.nitUsuario?.value,
      nombreCompleto: this.nombreCompleto?.value,
      emailInstitucional: this.emailInstitucional?.value,
      cargo: this.cargo?.value,
      activo: this.activo?.value,
      dobleFactorObligatorio: this.dobleFactorObligatorio?.value
    }

    if (this.esEdicion) {
      this.http.modificarUsuario(usuarioModelo).subscribe({
        next: (resultado: IRespuestaHttp) => { this.administrarRespuesta(resultado); },
        error: error => {
          this.bloquearBotones = false;
          Notify.failure(error.message);
        },
        complete: () => { this.bloquearBotones = false; }
      })
    } else {
      this.http.insertarUsuario(usuarioModelo).subscribe({
        next: (resultado: IRespuestaHttp) => { this.administrarRespuesta(resultado); },
        error: error => {
          this.bloquearBotones = false;
          Notify.failure(error.message);
        },
        complete: () => { this.bloquearBotones = false; }
      })
    }
  }

  private administrarRespuesta(resultado:IRespuestaHttp) : void {
    if(resultado.estado === EstadosHttp.success) {
      this.bsModalRef.hide();
      this.eventoGuardarUsuario.emit(resultado.resultado)
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

  public mostrarAlertaDobleFactor() {
    const esDobleFactor = this.dobleFactorObligatorio?.value;
    if (esDobleFactor) {
      Swal.fire({
        icon: "warning",
        text: "Al desactivar esta opción, el usuario esta expuesto al inicio de sesión desde el sistema SAU, sin un metodo de autorización propio y puede estar expuesto a suplantación de identidad. ¿desea continuar?",
        showCancelButton: true,
        confirmButtonText: 'Si',
        cancelButtonText: 'No',
        customClass: {
          cancelButton: 'btn-danger'
        }
      }).then((result) => {
        if (result.isConfirmed) {
          // Se autoriza que la opcion de doble factor esta inhabilitada
        } else {
          this.dobleFactorObligatorio?.setValue(true);
        }
      })
    }
  }



  public get nitUsuario() {return this.FormUsuario.get('nitUsuario');}
  public get nombreCompleto() {return this.FormUsuario.get('nombreCompleto');}
  public get emailInstitucional() {return this.FormUsuario.get('emailInstitucional');}
  public get cargo() {return this.FormUsuario.get('cargo');}
  public get dobleFactorObligatorio() {return this.FormUsuario.get('dobleFactorObligatorio');}
  public get activo() {return this.FormUsuario.get('activo');}
}
