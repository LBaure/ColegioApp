import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { IListaOpcionMenu } from 'src/app/expedientes-web/interfaces/seguridad/opciones-menu';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { SeguridadService } from '../../seguridad.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { Notify } from 'notiflix';
import Swal from 'sweetalert2';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';

@Component({
  selector: 'app-modal-opciones-menu',
  templateUrl: './modal-opciones-menu.component.html',
  styleUrls: ['./modal-opciones-menu.component.css']
})
export class ModalOpcionesMenuComponent implements OnInit, OnDestroy {
  public tituloModal: string = "";
  public isEdit : boolean = true;
  public opcionMenu: IListaOpcionMenu = {} as IListaOpcionMenu;
  public listaOpcionesMenu : IListaOpcionMenu[] = [];
  public EventoGuardar = new EventEmitter<IListaOpcionMenu>();

  formOpcionMenu: FormGroup = {} as FormGroup;
  loading : boolean = false;
  iconMenu : string = "";
  itemsOpcionesMenu : IListaOpcionMenu[] = [];
  itemsOpcionesMenuSelect : IListaOpcionMenu[] = [];
  itemsOpcionesPrevizualizar : IListaOpcionMenu[] = [];
  itemsActualizar : any[] = [];
  servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;


  constructor(
    private fb: FormBuilder,
    public bsModalRef: BsModalRef,
    private http: SeguridadService,
    private resizeService : ResizeService
  ) { this.formOpcionMenu = this.obtenerControlesFormulario(); }

  ngOnInit(): void {

    if(this.opcionMenu.idOpcionMenu) {
      this.formOpcionMenu.patchValue(this.opcionMenu);
      this.iconMenu = (this.opcionMenu.icono) ? this.opcionMenu.icono : '';
    }
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(screenSize => {
      this.mq = screenSize;
      if (screenSize === SCREEN_SIZE.XS || screenSize === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    })

    this.obtenerOpcionesMenu();
  }

  private obtenerControlesFormulario(): FormGroup {
    const group: FormGroup = this.fb.group({
      idOpcionMenu: [0],
      idOpcionMenuPadre: [0],
      nombre: [null, Validators.required],
      descripcion: [null, Validators.required],
      icono: [null],
      url: [null],
      orden: [1, [Validators.required, Validators.pattern('^([1-9]|[1-9][0-9])$')]],
      activo: [1, Validators.required],
      mostrarMenu: [true]
    })
    return group;
  }

  public obtenerOpcionesMenu() {
    if (this.listaOpcionesMenu.length) {
      let datos = this.listaOpcionesMenu.filter((el) => el.url == null)
      this.itemsOpcionesMenuSelect = [...datos];
      this.itemsOpcionesMenu = [...this.listaOpcionesMenu];
      if (this.isEdit) {
        let opcionesMenuCombo = datos.filter((el) =>  el.idOpcionMenu !== this.opcionMenu.idOpcionMenu)
        this.itemsOpcionesMenuSelect = [...opcionesMenuCombo]
        this.filtrar("init");
      }
    }
  }

  filtrar(inputMethod: string) {
    /**
     * Cada vez que cambie algun valor del Menu padre, se asignara vacio a la variable de tipo array
     * @itemsOpcionesPrevizualizar la cual contiene la Lista de opciones del menú
     */
    this.itemsOpcionesPrevizualizar = [];
    this.itemsActualizar = [];

    // Se obtiene el valor del campo Menu Padre
    const idOpcionMenuPadre = this.formOpcionMenu.controls['idOpcionMenuPadre'].value;
    if (!idOpcionMenuPadre) return; // Si no se encuentra termina la instruccion.

    /**
     * Se obtienen todas las opciones de menu
     * @copiaOpciones variable de tipo string, contiene una copia del array @itemsOpcionesMenu convertido en string para no mutar su valor
     * @listOpcion contiene los valores transforamdos en array de la copia de @itemsOpcionesMenu
     * @data contiene la informacion de todo el objeto padre seleccionado
     * @dataChildMessy contiene la informacion de todos los hijos desordenados del menu padre seleccionado
     * @dataChild contiene la informacion de todos los hijos ordenados.
     */
    let copiaOpciones = JSON.stringify(this.itemsOpcionesMenu);
    let listOpcion : IListaOpcionMenu[] = JSON.parse(copiaOpciones);

    const data = listOpcion.filter(element => element.idOpcionMenu === Number(idOpcionMenuPadre));
    const dataChildMessy = listOpcion.filter(element => element.idOpcionMenuPadre === Number(idOpcionMenuPadre));
    const dataChild = dataChildMessy.sort((x, y) => x.orden - y.orden);

    /**
     * Se agrega el objeto padre a la primer posision del array que contiene la lista de menu a previsualizar
     */
    this.itemsOpcionesPrevizualizar.push(data[0]);

    /**
     * Se valida si el campo Nombre Opcion @nombre del formulario contiene valor,
     * si es verdadero se crea un objeto nuevo @ObjetoActualizar el cual se va a agregar al array
     * que contiene los hijos del menu padre seleccionado @dataChildMessy
     */
    if (this.nombre?.value && !this.isEdit) {
      let ObjetoActualizar : IListaOpcionMenu = {
        idOpcionMenu: 0,
        nombre: this.nombre.value,
        orden: this.orden?.value
      }
      /**
       * Se valida si existe el numero de orden ingresado en el formulario con los menus hijos existentes.
       * Si existe el orden:
       *  -> El orden del formulario se mantiene
       *  -> Los ordenes mayores se les incrementa una posicion
       */
      let existeOrden = dataChild.find((obj) => obj.orden === ObjetoActualizar.orden)
      if(existeOrden) {
        dataChild.forEach((el) => {
          if (el.orden >= ObjetoActualizar.orden) {
            el.orden = el.orden + 1;
            this.itemsActualizar.push(el);
          }
        })
      }
      dataChild.push(ObjetoActualizar)
    }


    /**
     * Validación para cuando una opcion se va a modificar
     * @var idOpcionMenu asignamos el id o llave primaria del registro a modificar
     * @var numeroOrden asignamos el valor del campo orden del formulario
     * @var objOpcion Obtenemos el objeto que estamos modificando mediante el @var idOpcionMenu, para luego
     * cambiarle el valor a la propiedad orden por la @var numeroOrden
     * @var ordenAnterior Se asigna el valor del orden del registro antes de modificarlo y asignar el nuevo valor
     */
    if (this.isEdit) {

      let idOpcionMenu : number = this.idOpcionMenu?.value;
      let numeroOrden : number = this.orden?.value;
      let objOpcion = dataChild.find(el => el.idOpcionMenu === idOpcionMenu);

      if(objOpcion) {
        objOpcion.orden = numeroOrden;
      } else {
        let newChildOption = listOpcion.find(el => el.idOpcionMenu === idOpcionMenu);
        if (newChildOption) {
          newChildOption.idOpcionMenuPadre = Number(idOpcionMenuPadre);
          dataChild.push(newChildOption);
        }
      }

      let existeOrden = dataChild.find((obj) => obj.orden === this.orden?.value && obj.idOpcionMenu != idOpcionMenu);
      const orderedList = dataChild.sort((x, y) => x.orden - y.orden);

      if(existeOrden) { // Si existe un registro con el orden nuevo se recorre todo el arbol o lista hasta encontrarlo
        let posicion : number = 1;
        orderedList.forEach((el) => {

          if (el.idOpcionMenu != idOpcionMenu) {
            if (posicion === numeroOrden) {
              posicion += 1;
            }
            el.orden = posicion;
            posicion++;
            this.itemsActualizar.push(el);
          }
        })
      }


    }

    /**
     * @objChilds es una variable de tipo array que contien los hijos del menu seleccionado
     * ordenados ascendentemente y que posteriormente se asigna a la primer posicion del array principal
     * @itemsOpcionesPrevizualizar en su propiedad opciones.
     */
    let objChilds : IListaOpcionMenu[] =  dataChild.sort((x, y) => x.orden - y.orden);
    this.itemsOpcionesPrevizualizar[0].opciones = objChilds;

  }


  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
    this.bsModalRef.hide();
  }

   /**
   * btnAceptar
   */
   public btnAceptar() {
    if (!this.formOpcionMenu.valid) {
      Object.values(this.formOpcionMenu.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    this.loading = true;

    const reqOpcionMenu: IListaOpcionMenu = {
      idOpcionMenu: this.idOpcionMenu?.value,
      idOpcionMenuPadre: this.idOpcionMenuPadre?.value,
      nombre: this.nombre?.value,
      descripcion: this.descripcion?.value,
      icono: this.icono?.value,
      url: this.url?.value,
      orden: this.orden?.value,
      activo: this.activo?.value,
      opciones: this.itemsActualizar,
      mostrarMenu: this.mostrarMenu?.value
    }

    if (!this.isEdit) {
      this.http.insertarRegistroOpcionMenu(reqOpcionMenu).subscribe({
        next: (data: IRespuestaHttp) => { this.administrarRespuesta(data)},
        error: (error) => {
          Notify.failure(error.message);
          this.loading = false;
        },
        complete: () => { this.loading = false; }
      })
    } else {
      this.http.actualizarRegistroOpcionMenu(reqOpcionMenu).subscribe({
        next: (data: IRespuestaHttp) => { console.log("actualizacion: ", data);
         this.administrarRespuesta(data)},
        error: (error) => {
          Notify.failure(error.message);
          this.loading = false;
         },
        complete: () => { this.loading = false; }
      })
    }
  }

  private administrarRespuesta(resultado:IRespuestaHttp) : void {
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

  public get idOpcionMenu() {return this.formOpcionMenu.get('idOpcionMenu');}
  public get idOpcionMenuPadre() {return this.formOpcionMenu.get('idOpcionMenuPadre');}
  public get nombre() {return this.formOpcionMenu.get('nombre');}
  public get descripcion() {return this.formOpcionMenu.get('descripcion');}
  public get icono() {return this.formOpcionMenu.get('icono');}
  public get url() {return this.formOpcionMenu.get('url');}
  public get orden() {return this.formOpcionMenu.get('orden');}
  public get activo() {return this.formOpcionMenu.get('activo');}
  public get mostrarMenu() {return this.formOpcionMenu.get('mostrarMenu');}


}
