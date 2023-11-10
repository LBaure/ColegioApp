import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IBreadcrumb } from 'src/app/componentes-html/interfaces/models';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { AuthService } from 'src/app/utils/auth.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { SeguridadService } from '../seguridad.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { IRol, IRolOpcionMenu, IRolOpcionMenuDetalle } from 'src/app/expedientes-web/interfaces/seguridad/rol';
import { Router } from '@angular/router';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { Notify } from 'notiflix';
import { ModalRolesOpcionMenuComponent } from '../componentes/modal-roles-opcion-menu/modal-roles-opcion-menu.component';
import { IListaOpcionMenu } from 'src/app/expedientes-web/interfaces/seguridad/opciones-menu';
import Swal from 'sweetalert2';
import { AdministracionService } from '../../administracion/administracion.service';
import { ICatalogoOpcionMenu } from 'src/app/expedientes-web/interfaces/administracion/catalogo';

@Component({
  selector: 'app-roles-opciones-menu',
  templateUrl: './roles-opciones-menu.component.html',
  styleUrls: ['./roles-opciones-menu.component.css']
})
export class RolesOpcionesMenuComponent implements OnInit, OnDestroy {
  private servicesPantalla$ : Subscription = new Subscription;
  mq? : SCREEN_SIZE;
  headers: IHeader[] = [];
  permisos: IPermisosUsuario = {} as IPermisosUsuario;
  itemsRoutes : IBreadcrumb[] = [
    { title: 'Roles', path: '..'},
    { title: 'Opciones Menú', path: '', disabled: true }
  ]
  localStorageItem : string | null = null;
  rolLocalStorage: IRol = {} as IRol;
  listadoRolOpcionesMenu: IRolOpcionMenuDetalle[] = [];
  listadoRolOpcionesMenuFiltrados: IRolOpcionMenuDetalle[] = [];
  page : number = 1;
  itemsPage : number = 10;
  bsModalOpcionMenu: BsModalRef | undefined;
  opcionesMenu: ICatalogoOpcionMenu[] = [];
  _filtrarBusqueda: string = "";
  resultadobusqueda: boolean = false;

  get filtrarBusqueda() {
    return this._filtrarBusqueda;
  }
  set filtrarBusqueda (valor: string) {
    this._filtrarBusqueda = valor;
    this.listadoRolOpcionesMenuFiltrados = this.filtrarListado(valor);
    this.resultadobusqueda = this.listadoRolOpcionesMenuFiltrados.length ? false : true
    if (!valor) this.resultadobusqueda = false;
  }

  constructor(
    private authService : AuthService,
    private resizeService : ResizeService,
    private http : SeguridadService,
    private modalService: BsModalService,
    private router : Router,
    private httpCatalogo : AdministracionService

  ) {
    this.localStorageItem = localStorage.getItem("Rol");
    this.permisos = this.authService.obtenerPermisos();
    this.headers = this.obtenerEncabezadoTabla();
  }
  ngOnDestroy(): void {
    localStorage.removeItem("Rol");
    this.bsModalOpcionMenu?.hide();
  }

  filtrarListado(termino : string) {
    if (this.listadoRolOpcionesMenu.length === 0 || this.filtrarBusqueda === '') {
      return this.listadoRolOpcionesMenu;
    } else {
      return this.listadoRolOpcionesMenu.filter((option) => {
        return option.opcionMenu?.toString().trim().toLowerCase().includes(termino.trim().toLowerCase()) ||
        option.descripcionOpcion?.toString().trim().toLowerCase().includes(termino.trim().toLowerCase())
      })
    }
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})
    if (this.localStorageItem) {
      this.rolLocalStorage =  JSON.parse(this.localStorageItem);
      this.obtenerOpcionesMenuPorRol();
      this.obtenerOpcionesMenu();
    } else {
      this.router.navigate(['admin/seguridad', 'roles']);
    }
  }

  private obtenerEncabezadoTabla() : IHeader[] {
    let encabezados : IHeader [] = [
      { text: 'Nombre', width: '20%', align: 'center'},
      { text: 'Descripcion', width: '30%', align: 'center'},
      { text: 'Consulta', width: '10%', align: 'center'},
      { text: 'Inserta', width: '10%', align: 'center'},
      { text: 'Modifica', width: '10%', align: 'center'},
      { text: 'Elimina', width: '10%', align: 'center'},
    ]
    if (this.permisos.elimina || this.permisos.modifica) {
      encabezados.push({ text: 'Acciones', width: '10%', align: 'center'},)
    }
    return encabezados;
  }

  private obtenerOpcionesMenuPorRol() {
    this.http.obtenerOpcionesMenuPorRol(this.rolLocalStorage).subscribe({
      next: (resultado: IRespuestaHttp ) => {
        console.log("obtenerOpcionesMenuPorRol", resultado);

        if (resultado.estado === EstadosHttp.success) {
          let datos : IRolOpcionMenu[] = resultado.resultado;
          this.listadoRolOpcionesMenu = [...datos];
          this.listadoRolOpcionesMenuFiltrados = [...datos];
        }
      },
      error: (error) => { Notify.failure(error.message)}
    })
  }

  /**
   * abrirModalAgregarOpcionMenu
   */
  public abrirModalAgregarOpcionMenu(esEdicion:boolean, OpcionMenu?: IRolOpcionMenuDetalle): void {

    let listaOpcionesMenu = this.filtrarOpcionesMenu(esEdicion)
    console.log("abrirModalAgregarOpcionMenu", listaOpcionesMenu);
    const initialState = {
      tituloModal: esEdicion ? "Actualizar permisos" : "Agregar permisos",
      esEdicion: esEdicion,
      listaDesplegableOpcionesMenu: listaOpcionesMenu,
      rolActualizar: esEdicion ? OpcionMenu : {} as IRolOpcionMenuDetalle,
      idRolLocalStorage: this.rolLocalStorage.idRol
    }
    this.bsModalOpcionMenu = this.modalService.show(ModalRolesOpcionMenuComponent, { initialState, class: 'modal-md modal-dialog-centered', keyboard: true, backdrop: 'static' });
    this.bsModalOpcionMenu.content.eventoGuardar.subscribe( (opcionesMenu : IRolOpcionMenuDetalle[]) => {
      if (opcionesMenu.length) {
        this.listadoRolOpcionesMenu = opcionesMenu;
        this.listadoRolOpcionesMenuFiltrados = opcionesMenu;
      }
    })
  }

  private obtenerOpcionesMenu () {
    this.httpCatalogo.catalogoOpcionesMenu()
      .subscribe({
        next: (resultado: IRespuestaHttp) => {

          if (resultado.estado === EstadosHttp.success){
            this.opcionesMenu = [...resultado.resultado];
            console.log("******************obtenerOpcionesMenu***********", this.opcionesMenu);
          }
        },
        error: error => {
          Notify.failure(error.message)
        }
      });
  }

  private filtrarOpcionesMenu(esEdicion:boolean): ICatalogoOpcionMenu[] {
    let clonOpcionesMenu : ICatalogoOpcionMenu[] = [...this.opcionesMenu];
    if (esEdicion) return clonOpcionesMenu;

    console.log("clonOpcionesMenu", this.listadoRolOpcionesMenu, this.opcionesMenu);


    this.listadoRolOpcionesMenu.forEach(element => {
      var existe = this.opcionesMenu.find(el => el.idOpcionMenu === element.idOpcionMenu)
      var indiceClon = clonOpcionesMenu.findIndex(elemento => elemento.idOpcionMenu === existe?.idOpcionMenu);
      if (indiceClon != null || indiceClon != undefined) {
        clonOpcionesMenu.splice(indiceClon,1);
      }
    });
    return clonOpcionesMenu;
  }

  /**
   * eliminarRolOpcionMenu
   */
  public eliminarRolOpcionMenu(opcionMenu: IRolOpcionMenuDetalle): void {
    Swal.fire({
      icon: "warning",
      html: `¿Desea eliminar la opción de menú <strong>${opcionMenu.opcionMenu}</strong>?`,
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.eliminarRolOpcionMenu(opcionMenu)
          .subscribe({
            next: (resultado) => { this.administrarRespuesta(resultado) },
            error: error => { Notify.failure(error.message); }
          })
      }
    })
  }



  private administrarRespuesta(respuesta:IRespuestaHttp) : void {
    if (respuesta.estado === EstadosHttp.success) {
      let usuarios :IRolOpcionMenuDetalle[] = respuesta.resultado;
      this.listadoRolOpcionesMenu = usuarios;
      this.listadoRolOpcionesMenuFiltrados = usuarios;
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
