import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { IRol } from 'src/app/expedientes-web/interfaces/seguridad/rol';
import { AuthService } from 'src/app/utils/auth.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../../seguridad.service';
import { AdministracionService } from '../../../administracion/administracion.service';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { ICatalogoGeneral } from 'src/app/expedientes-web/interfaces/administracion/catalogo';
import Swal from 'sweetalert2';
import { IUsuarioRoles } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { Notify } from 'notiflix';

@Component({
  selector: 'app-modal-agregar-roles-usuario',
  templateUrl: './modal-agregar-roles-usuario.component.html',
  styleUrls: ['./modal-agregar-roles-usuario.component.css']
})
export class ModalAgregarRolesUsuarioComponent implements OnInit {
  titulo: string = "Agregar roles";
  nitUsuario: string = "";
  permisos: IPermisosUsuario = {} as IPermisosUsuario;
  roles: ICatalogoGeneral[] = [];
  setCheck = new Array(2).fill(false);
  rolesUsuario: number[]=[];
  private servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;
  loading: boolean = false;



  constructor(
    public bsModalRef: BsModalRef,
    private http: SeguridadService,
    private httpCatalogo: AdministracionService,
    private authService : AuthService,
    private resizeService : ResizeService,
  ) {
    this.permisos = this.authService.obtenerPermisos();

  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(screenSize => {
      this.mq = screenSize;
      if (screenSize === SCREEN_SIZE.XS || screenSize === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    })

    this.administrarRolesUsuario();
  }

  public administrarRolesUsuario(): void {
    this.setCheck = new Array(this.roles.length).fill(false);
    if (this.rolesUsuario.length) {
      this.rolesUsuario.forEach((element:any, index:any) => {
        this.administrarCambioRoles(index, element);
      });
    }
  }

  administrarCambioRoles = (position : any, element : number) => {
    const comprobarEstadoRol = this.setCheck.map((item, index) => {
      let obj = this.roles[index]
      return obj.id === element ? !item : item
    }
    );
    this.setCheck = comprobarEstadoRol;
  };


  public AgregarRolUsuario(value: any) {
    const resultado = this.rolesUsuario.find(rol => rol === value );
    if (resultado === undefined)  {
      this.rolesUsuario.push(value)
    } else {
      for(let i = 0; i < this.rolesUsuario.length; i++){
        if (value === this.rolesUsuario[i]) {
          this.rolesUsuario.splice(i, 1)
        }
      }
    }
  }

  /**
   * guardarRolesUsuario
   */
  public guardarRolesUsuario() {
    if(!this.rolesUsuario.length) {
      Swal.fire({
        title: 'Campos Obligatorios',
        text: 'Seleccione como mínimo un rol, para asignarle al usuario.',
        icon: 'warning',
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Aceptar'
      });
      return;
    }

    if (!this.permisos.inserta || !this.permisos.modifica) {
      Swal.fire({
        title: 'Permisos',
        text: 'No tiene permisos para realizar esta acción.',
        icon: 'warning',
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Aceptar'
      });
      return;
    }

    const reqUsuarioRol : IUsuarioRoles = {
      nitUsuario: this.nitUsuario,
      roles: JSON.stringify(this.rolesUsuario)
    }

    console.log("reqUsuarioRol", reqUsuarioRol);

    this.http.insertarRolesUsuario(reqUsuarioRol).subscribe({
      next: (resultado: IRespuestaHttp) => { this.administrarRespuesta(resultado) },
      error: (error) => {
        this.loading = false;
        Notify.failure(error.message) },
      complete: () => {}
    })

  }


  private administrarRespuesta(resultado:IRespuestaHttp) : void {
    if(resultado.estado === EstadosHttp.success) {
      this.bsModalRef.hide();
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

}
