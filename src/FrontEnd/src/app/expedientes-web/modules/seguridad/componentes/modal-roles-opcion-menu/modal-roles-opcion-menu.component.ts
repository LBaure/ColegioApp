import { Component, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { IListaOpcionMenu, IOpcionMenu } from 'src/app/expedientes-web/interfaces/seguridad/opciones-menu';
import { IRolOpcionMenu, IRolOpcionMenuDetalle } from 'src/app/expedientes-web/interfaces/seguridad/rol';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../../seguridad.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { Notify } from 'notiflix';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import Swal from 'sweetalert2';
import { ICatalogoOpcionMenu } from 'src/app/expedientes-web/interfaces/administracion/catalogo';

@Component({
  selector: 'app-modal-roles-opcion-menu',
  templateUrl: './modal-roles-opcion-menu.component.html',
  styleUrls: ['./modal-roles-opcion-menu.component.css']
})
export class ModalRolesOpcionMenuComponent implements OnInit {
  tituloModal: string = "";
  esEdicion: boolean = false;
  idRolLocalStorage: number = 0;
  listaDesplegableOpcionesMenu: ICatalogoOpcionMenu[] = [];
  rolActualizar: IRolOpcionMenuDetalle = {} as IRolOpcionMenuDetalle;
  eventoGuardar = new EventEmitter<IRolOpcionMenuDetalle[]>();

  private servicesPantalla$ : Subscription = new Subscription;
  public mq? : SCREEN_SIZE;
  public formRolOpcionMenu: FormGroup = {} as FormGroup;
  public loading : boolean = false;
  public opcionSeleccionada: string = "Seleccione una opción"



  constructor(
    private fb: FormBuilder,
    public bsModalRef: BsModalRef,
    private http: SeguridadService,
    private resizeService : ResizeService
  ) {
    this.formRolOpcionMenu = this.obtenerControlesFormulario();
  }

  private obtenerControlesFormulario(): FormGroup {
    const group: FormGroup = this.fb.group({
      idOpcionMenu: [null, Validators.required],
      idRol: [0],
      consulta: [false],
      modifica: [false],
      inserta: [false],
      elimina: [false]
    })
    return group;
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      this.mq = x;
      if (x === SCREEN_SIZE.XS || x === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    })
    this.asignarValores();
  }

  private asignarValores() {
    console.log("asignarValores", this.listaDesplegableOpcionesMenu);

    if (!this.esEdicion) {
      this.idRol?.setValue(this.idRolLocalStorage);
      return
    };
    this.formRolOpcionMenu.patchValue(this.rolActualizar);
    let opcionMenu = this.listaDesplegableOpcionesMenu.find(el => el.idOpcionMenu === this.idOpcionMenu?.value);
    if (opcionMenu) {
      this.cambiarValorSeleccionado(opcionMenu)
    }
  }

  /**
   * guardarOpcionMenu
   */
  public guardarOpcionMenu() {
    if (!this.formRolOpcionMenu.valid) {
      Object.values(this.formRolOpcionMenu.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    const solicitud : IRolOpcionMenu = {
      idRol: this.idRol?.value,
      idOpcionMenu: this.idOpcionMenu?.value,
      consulta: this.consulta?.value,
      modifica: this.modifica?.value,
      inserta: this.inserta?.value,
      elimina: this.elimina?.value
    }

    const metodoEjecutar = (this.esEdicion) ? 'modificarOpcionMenuPorRol' : 'insertarOpcionMenuPorRol';
    console.log("parmas", solicitud)
    console.log("metodoEjecutar", metodoEjecutar)

    this.http[metodoEjecutar](solicitud)
      .subscribe({
        next: (resultado) => { this.administrarRespuesta(resultado); },
        error: (error) => {
          Notify.failure(error.message);
          this.loading = false;
        }
      })
  }

  private administrarRespuesta(resultado: IRespuestaHttp) : void {
    this.loading = false;
    if(resultado.estado === EstadosHttp.success) {
      this.bsModalRef.hide();
      this.eventoGuardar.emit(resultado.resultado)
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

  cambiarValorSeleccionado(item : ICatalogoOpcionMenu) {
    if(!item) return;
    this.opcionSeleccionada = item.nombre;
    this.idOpcionMenu?.setValue(item.idOpcionMenu);
  }

  public get idRol() {return this.formRolOpcionMenu.get('idRol');}
  public get idOpcionMenu() {return this.formRolOpcionMenu.get('idOpcionMenu');}
  public get consulta() {return this.formRolOpcionMenu.get('consulta');}
  public get inserta() {return this.formRolOpcionMenu.get('inserta');}
  public get modifica() {return this.formRolOpcionMenu.get('modifica');}
  public get elimina() {return this.formRolOpcionMenu.get('elimina');}

}
