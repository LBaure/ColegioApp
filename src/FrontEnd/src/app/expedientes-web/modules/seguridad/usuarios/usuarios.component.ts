import { Component, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { AuthService } from 'src/app/utils/auth.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { ResizeService } from 'src/app/utils/resize.service';
import { Notify } from 'notiflix';
import { ModalAgregarRolesUsuarioComponent } from '../componentes/modal-agregar-roles-usuario/modal-agregar-roles-usuario.component';
import { ModalUsuarioAdminComponent } from '../componentes/modal-usuario-admin/modal-usuario-admin.component';
import { IUsuario } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { SeguridadService } from '../seguridad.service';
import { AdministracionService } from '../../administracion/administracion.service';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { ICatalogoGeneral } from 'src/app/expedientes-web/interfaces/administracion/catalogo';
import { BitacoraRolesUsuarioComponent } from '../componentes/bitacora-roles-usuario/bitacora-roles-usuario.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-usuarios',
  templateUrl: './usuarios.component.html',
  styleUrls: ['./usuarios.component.css']
})
export class UsuariosComponent implements OnInit, OnDestroy {
  private servicesPantalla$ : Subscription = new Subscription;
  headers: IHeader[] = [];
  permisos: IPermisosUsuario = {} as IPermisosUsuario;
  bsModalRolUsuario: BsModalRef | undefined;
  bsModalUser: BsModalRef | undefined;
  bsModalBitacora: BsModalRef | undefined;
  mq? : SCREEN_SIZE;
  _filtrarBusqueda: string = "";
  listadoUsuarios: IUsuario[] = [];
  listadoUsuariosFiltrados: IUsuario[] = [];
  page : number = 1;
  itemsPage : number = 10;
  resultadobusqueda: boolean = false;
  listaCatalogoRoles: ICatalogoGeneral [] = [];


  get filtrarBusqueda() {
    return this._filtrarBusqueda;
  }
  set filtrarBusqueda (valor: string) {
    this._filtrarBusqueda = valor;
    this.listadoUsuariosFiltrados = this.filtrarListado(valor);
    this.resultadobusqueda = this.listadoUsuariosFiltrados.length ? false : true
    if (!valor) this.resultadobusqueda = false;
  }

  constructor(
    private modalService: BsModalService,
    private authService : AuthService,
    private resizeService : ResizeService,
    private http : SeguridadService,
    private httpCatalogo: AdministracionService
  ) {
    this.permisos = this.authService.obtenerPermisos();
    this.headers = this.obtenerEncabezadoTabla();
  }
  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
    this.bsModalRolUsuario?.hide();
    this.bsModalBitacora?.hide();
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})
    this.obtenerUsuarios();
    this.obtenerRoles();
  }

  obtenerEncabezadoTabla() : IHeader[] {
    let encabezados : IHeader [] = [
      { text: 'Nombres', width: '17%', align: 'center'},
      { text: 'Correo Institucional', width: '12%', align: 'center'},
      { text: 'Correo personal', width: '12%', align: 'center'},
      { text: 'Puesto', width: '15%', align: 'center'},
      { text: 'Telefono', width: '8%', align: 'center'},
      { text: 'Fecha Registro', width: '8%'},
      { text: 'Estado', width: '6%', align: 'center'},
      { text: 'Roles', width: '6%', align: 'center' },
      { text: 'Bitácora', width: '6%', align: 'center' },
    ]
    if (this.permisos.elimina || this.permisos.modifica) {
      encabezados.push({ text: 'Acciones', width: '10%', align: 'center'},)
    }
    return encabezados;
  }

  filtrarListado(busqueda : string) {
    if (this.listadoUsuarios.length === 0 || this.filtrarBusqueda === '') {
      return this.listadoUsuarios;
    } else {
      return this.listadoUsuarios.filter((option) => {
        return option.nombreCompleto.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase()) ||
        option.nitUsuario?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())||
        option.cargo?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())||
        option.emailInstitucional?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())
      })
    }
  }

  obtenerUsuarios() {
    this.http.obtenerUsuarios().subscribe({
      next: (data) => {
        this.listadoUsuarios = [...data];
        this.listadoUsuariosFiltrados = [...data];
      },
      error: error => {
        Notify.failure(error.message)
      }
    });
  }

  async abrirModalAgregarRoles(usuario : IUsuario) {
    const initialState = {
      titleModal: "Asignación de roles",
      nitUsuario : usuario.nitUsuario,
      roles: this.listaCatalogoRoles,
      rolesUsuario: await this.obtenerRolesUsuarios(usuario)
    }
    this.bsModalRolUsuario = this.modalService.show(ModalAgregarRolesUsuarioComponent, { initialState, keyboard: true, backdrop: "static"})
  }

  private async obtenerRolesUsuarios(usuario: IUsuario) {
    let respuesta = await this.http.obtenerRolesUsuario(usuario.nitUsuario).toPromise();
    let roles: number[]=[];
    if (!respuesta) return roles;
    if (respuesta.resultado.length) {
      let lista = JSON.parse(respuesta.resultado[0].roles);
      roles = [...lista]
    }
    return roles;
  }

  private obtenerRoles () {
    this.httpCatalogo.catalogoRoles()
    .subscribe({
      next: (data: IRespuestaHttp) => {
        if (data.estado === EstadosHttp.success) {
          let result : ICatalogoGeneral[] = data.resultado;
          this.listaCatalogoRoles = result;
        }
      },
      error: error => { Notify.failure(error.message); }
    });

  }

  abrirModalAgregarUsuario(edicion : boolean = false, usuario?: IUsuario) {
    const initialState = {
      tituloModal: "Registro de Usuarios",
      esEdicion: edicion,
      usuarioModelo: usuario
    }

    this.bsModalUser = this.modalService.show(ModalUsuarioAdminComponent, { initialState, class: 'modal-md modal-dialog-centered', keyboard: true, backdrop: "static"});
    this.bsModalUser.content.eventoGuardarUsuario.subscribe( (data : IUsuario[]) => {
      if (data.length ){
        this.listadoUsuarios = [...data];
        this.listadoUsuariosFiltrados = [...data];
      }
    })
  }

  eliminarUsuario(usuario : IUsuario) {
    Swal.fire({
      icon: "warning",
      html: `¿Desea eliminar al usuario <strong>${ usuario.nombreCompleto }</strong>?`,
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.eliminarUsuario(usuario)
          .subscribe({
            next: (data) => { this.administrarRespuesta(data) },
            error: error => { Notify.failure(error.message) }
          })
      }
    })
  }

  abrirModalBitacoraRoles(usuario: IUsuario) {
    const initialState = {
      nitUsuario : usuario.nitUsuario,
      nombreUsuario: usuario.nombreCompleto
    }

    this.bsModalBitacora = this.modalService.show(BitacoraRolesUsuarioComponent, { initialState, class: 'modal-lg modal-dialog-centered', keyboard: true, backdrop: "static"})
  }

  private administrarRespuesta(respuesta:IRespuestaHttp) : void {
    if (respuesta.estado === EstadosHttp.success) {
      let usuarios :IUsuario[] = respuesta.resultado;
      this.listadoUsuarios = usuarios;
      this.listadoUsuariosFiltrados = usuarios;
    }

    if (this.mq === SCREEN_SIZE.XS || this.mq === SCREEN_SIZE.SM) {
      Notify[respuesta.estado as keyof typeof Notify](respuesta.mensaje, {
        position: 'center-center',
        fontSize: 'var(--bs-body-font-size)',
        clickToClose: true,
      })
    }
    else {
      Swal.fire({
        icon: respuesta.icono,
        title: respuesta.titulo,
        text: respuesta.mensaje,
        showCloseButton: true
      });
    }
  }
}
