import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/utils/auth.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { SeguridadService } from '../seguridad.service';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { Subscription } from 'rxjs';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { IListaOpcionMenu } from 'src/app/expedientes-web/interfaces/seguridad/opciones-menu';
import { Notify } from 'notiflix';
import { IOpcionesMenu } from 'src/app/expedientes-web/interfaces/roles';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import Swal from 'sweetalert2';
import { ModalOpcionesMenuComponent } from '../componentes/modal-opciones-menu/modal-opciones-menu.component';
import { Functions } from 'src/app/utils/functions/functions';

@Component({
  selector: 'app-opciones-menu',
  templateUrl: './opciones-menu.component.html',
  styleUrls: ['./opciones-menu.component.css']
})
export class OpcionesMenuComponent implements OnInit {
  private servicesPantalla$ : Subscription = new Subscription;
  headers: IHeader[] = [];
  permisos: IPermisosUsuario = {} as IPermisosUsuario;
  mq? : SCREEN_SIZE;
  _filtrarBusqueda: string = "";
  listadoOpcionesMenu: IListaOpcionMenu[] = [];
  listadoOpcionesMenuFiltrados: IListaOpcionMenu[] = [];
  ascendente : boolean = false;
  page : number = 1;
  itemsPage : number = 10;
  bsModalOpcionMenu: BsModalRef | undefined;
  resultadobusqueda: boolean = false;

  get filtrarBusqueda() {
    return this._filtrarBusqueda;
  }
  set filtrarBusqueda (valor: string) {
    this._filtrarBusqueda = valor;
    this.listadoOpcionesMenuFiltrados = this.filtrarListado(valor);
    this.resultadobusqueda = this.listadoOpcionesMenuFiltrados.length ? false : true
    if (!valor) this.resultadobusqueda = false;
  }


  constructor(
    private modalService: BsModalService,
    private fn : Functions,
    private authService : AuthService,
    private resizeService : ResizeService,
    private http : SeguridadService
  ) {
    this.permisos = this.authService.obtenerPermisos();
    this.headers = this.obtenerEncabezadoTabla();
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})
    this.obtenerListadoOpcionesMenu();
  }

  obtenerEncabezadoTabla() : IHeader[] {
    let encabezados : IHeader [] = [
      { text: 'Descripción', width: '35%', align: 'center'},
      { text: 'Menú Padre', width: '15%', align: 'center'},
      { text: 'Ruta', width: '20%', align: 'center'},
      { text: 'Orden', width: '10%', align: 'center', value: 'orden', orderBy: true },
      { text: 'Estado', width: '10%', align: 'center'}
    ]
    if (this.permisos.elimina || this.permisos.modifica) {
      encabezados.push({ text: 'Acciones', width: '10%', align: 'center'},)
    }
    return encabezados;
  }

  private obtenerListadoOpcionesMenu() : void {
    this.http.obtenerOpcionesMenu()
      .subscribe({
        next: (data) => {
          console.log("todos los datos", data);

          this.listadoOpcionesMenu = [...data];
          this.listadoOpcionesMenuFiltrados = [...data];
        },
        error: error => {
          Notify.failure(error.message)
        }
      });
  }

  /**
   * filtrarListado
   * filtro de busqueda, al agregar algun caracter en el campo Buscar, este filtra los resultados de
   * la lista y retorna unicamente los valores que coincidan con la cadena de caracteres ingresada.
   * @param filterTerm tipo string
   */
  public filtrarListado(filterTerm : string) {
    if (this.listadoOpcionesMenu.length === 0 || this.filtrarBusqueda === '') {
      return this.listadoOpcionesMenu;
    } else {
      return this.listadoOpcionesMenu.filter((option) => {
        return option.nombre.toString().trim().toLowerCase().includes(filterTerm.trim().toLowerCase()) ||
        option.nombreMenuPadre?.toString().trim().toLowerCase().includes(filterTerm.trim().toLowerCase())||
        option.url?.toString().trim().toLowerCase().includes(filterTerm.trim().toLowerCase())
      })
    }
  }

  /**
   * ordenarPorEncabezado
   * Ordena los items en orden descendente y ascendente
   * @param value
   * @returns
   */
  public ordenarPorEncabezado(value?: string) {
    if(!value) return;
    this.ascendente = !this.ascendente
    let head = value as keyof IListaOpcionMenu;
    let order : any = this.ascendente ? 'asc' : 'desc';
    this.listadoOpcionesMenuFiltrados.sort(this.fn.createCompareFn(head, order));
  }

  /**
   * agregarNuevaOpcion
   */
  public abrirModalOpcionMenu(esEdicion : boolean, item?: IListaOpcionMenu) {
    console.log("abrirModalOpcionMenu", this.listadoOpcionesMenu);

    const initialState = {
      tituloModal: `Registro de Opciones del Menú`,
      isEdit : esEdicion,
      opcionMenu: !esEdicion ? {} as IListaOpcionMenu : item,
      listaOpcionesMenu: this.listadoOpcionesMenu
    }

    this.bsModalOpcionMenu = this.modalService.show(ModalOpcionesMenuComponent, { initialState, class: 'modal-lg modal-dialog-centered', keyboard: true})
    this.bsModalOpcionMenu.content.EventoGuardar.subscribe((data:IListaOpcionMenu[]) => {
      if (data.length ){
        this.listadoOpcionesMenu = [...data];
        this.listadoOpcionesMenuFiltrados = [...data];
      }
    })
  }

  /**
   * btnEliminar
   */
  public btnEliminar(item : IOpcionesMenu) {
    Swal.fire({
      icon: "warning",
      title: '¿Desea eliminar el registro?',
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.eliminarRegistroOpcionMenu(item).subscribe({
          next: (data) => {
            if(data.estado === EstadosHttp.success) {
              Swal.fire({
                text: data.mensaje,
                icon: data.icono
              })
              this.listadoOpcionesMenu = [...data.resultado];
              this.listadoOpcionesMenuFiltrados = [...data.resultado];
            }
          },
          error: error => { Notify.failure(error.message) }
        })
      }
    })
  }

  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
    this.bsModalOpcionMenu?.hide();
  }


}
