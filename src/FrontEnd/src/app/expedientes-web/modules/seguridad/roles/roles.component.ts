import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { IRol } from 'src/app/expedientes-web/interfaces/seguridad/rol';
import { AuthService } from 'src/app/utils/auth.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../seguridad.service';
import { Notify } from 'notiflix';
import { Subscription } from 'rxjs';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ModalRolesComponent } from '../componentes/modal-roles/modal-roles.component';
import { IEstadosAdminstrativo } from 'src/app/expedientes-web/interfaces/estados-admin';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit, OnDestroy  {
  private servicesPantalla$ : Subscription = new Subscription;
  headers: IHeader[] = [];
  permisos: IPermisosUsuario = {} as IPermisosUsuario;

  mq? : SCREEN_SIZE;
  _filtrarBusqueda: string = "";
  listadoRolesFiltrados: IRol[] = [];
  listadoRoles: IRol[] = [];
  listadoEstados: IEstadosAdminstrativo[] = [];
  page : number = 1;
  itemsPage : number = 10;
  bsModalRoles: BsModalRef | undefined;
  resultadobusqueda: boolean = false;


  get filtrarBusqueda() {
    return this._filtrarBusqueda;
  }
  set filtrarBusqueda (valor: string) {
    this._filtrarBusqueda = valor;
    this.listadoRolesFiltrados = this.filtrarListado(valor);
    this.resultadobusqueda = this.listadoRolesFiltrados.length ? false : true
    if (!valor) this.resultadobusqueda = false;
  }



  constructor(
    private authService : AuthService,
    private resizeService : ResizeService,
    private http : SeguridadService,
    private modalService: BsModalService,
  ) {
    this.permisos = this.authService.obtenerPermisos();
    this.headers = this.obtenerEncabezadoTabla();
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})
    this.obtenerRoles();
    this.obtenerCatalogoEstados();
  }

  private obtenerEncabezadoTabla() : IHeader[] {
    let encabezados : IHeader [] = [
      { text: '#', width: '10%', align: 'right'},
      { text: 'Nombre', width: '20%', align: 'center'},
      { text: 'Descripción', width: '40%', align: 'center'},
      { text: 'Estado', width: '10%', align: 'center'},
      { text: 'Opciones Menú', width: '10%', align: 'center'}
    ]
    if (this.permisos.elimina || this.permisos.modifica) {
      encabezados.push({ text: 'Acciones', width: '10%', align: 'center'},)
    }
    return encabezados;
  }


  filtrarListado(termino : string) {
    if (this.listadoRoles.length === 0 || this.filtrarBusqueda === '') {
      return this.listadoRoles;
    } else {
      return this.listadoRoles.filter((option) => {
        return option.nombre.toString().trim().toLowerCase().includes(termino.trim().toLowerCase()) ||
        option.descripcion?.toString().trim().toLowerCase().includes(termino.trim().toLowerCase())
      })
    }
  }

  obtenerRoles() {
    this.http.obtenerRoles().subscribe({
      next: (resultado: IRespuestaHttp ) => {
        if (resultado.estado === EstadosHttp.success) {
          let datos : IRol[] = resultado.resultado;
          this.listadoRoles = datos;
          this.listadoRolesFiltrados = datos;
        }
      },
      error: (error) => { Notify.failure(error.message)}
    })
  }
  private obtenerCatalogoEstados (): void {
    this.http.getConfig().subscribe((estados) => {
      this.listadoEstados = [...estados]
    })
  }

  public abrirModalAgregarRol(esEdicion : boolean, rol?: IRol) {
    const initialState = {
      tituloModal: `Registro de Roles`,
      esEdicion : esEdicion,
      rol: !esEdicion ? {} as IRol : rol,
      listadoEstados: this.listadoEstados
    }

    this.bsModalRoles = this.modalService.show(ModalRolesComponent, { initialState, class: 'modal-md modal-dialog-centered', keyboard: true})
    this.bsModalRoles.content.EventoGuardar.subscribe( (roles : IRol[]) => {
      if (roles.length) {
        this.listadoRoles = [...roles];
        this.listadoRolesFiltrados = [...roles];
      }
    })
  }

  public eliminarRol(rol: IRol) {
    Swal.fire({
      icon: "warning",
      html: `¿Desea eliminar el rol <strong>${rol.nombre}</strong>?`,
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.eliminarRol(rol)
          .subscribe({
            next: (resultado) => {
              if(resultado.estado === EstadosHttp.success) {
                this.listadoRoles = [...resultado.resultado];
                this.listadoRolesFiltrados = [...resultado.resultado];
              }
              Swal.fire({
                title: resultado.titulo,
                text: resultado.mensaje,
                icon: resultado.icono
              })
            },
            error: error => { Notify.failure(error.message); }
          })
      }
    })

  }
  public irOpcionesMenu(rol: IRol) {
    localStorage.setItem("Rol",  JSON.stringify(rol));
  }

  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
    this.bsModalRoles?.hide();
  }

}
