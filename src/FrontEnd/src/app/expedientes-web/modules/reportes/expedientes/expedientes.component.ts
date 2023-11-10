import { Component, ElementRef, OnInit } from '@angular/core';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { IPermisosUsuario } from 'src/app/interfaces/usuario';
import { AuthService } from 'src/app/utils/auth.service';
import { ReportesService } from '../reportes.service';
import { IFiltrosExpedientesPorTipo } from 'src/app/expedientes-web/interfaces/dashboard/filtros-expedientes-por-tipo';
import { Notify } from 'notiflix';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ModalExpedientesInfoComponent } from '../components/modal-expedientes-info/modal-expedientes-info.component';
import { ModalFiltrosAvanzadosComponent } from '../components/modal-filtros-avanzados/modal-filtros-avanzados.component';
import { ITipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';
import { IFiltrosAvanzadosPorTipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/filtros-avanzados-por-tipo-expediente';
import { Subscription, map } from 'rxjs';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { IBreadcrumb } from 'src/app/componentes-html/interfaces/models';
import { Functions } from 'src/app/utils/functions/functions';

@Component({
  selector: 'app-expedientes',
  templateUrl: './expedientes.component.html',
  styleUrls: ['./expedientes.component.css']
})
export class ExpedientesComponent implements OnInit {
  titulo: string = '';
  bsModalRef?: BsModalRef;
  headers: IHeader[] = [];
  _filtrarBusqueda: string = "";
  page : number = 1;
  itemsPage : number = 10;
  listadoExpedientesInicial: any[] = [];
  listadoExpedientes: any[] = [];
  resultadobusqueda: boolean = false;
  permisos: IPermisosUsuario = {} as IPermisosUsuario;
  listadoTiposExpedientes: ITipoExpediente[] = [];
  paramFiltrosUrl: IFiltrosAvanzadosPorTipoExpediente = {} as IFiltrosAvanzadosPorTipoExpediente;
  // paramFiltrosUrl: Partial<IFiltrosAvanzadosPorTipoExpediente> = {};
  private routeSub: Subscription = new Subscription;

  itemsRoutes : IBreadcrumb[] = [
    { title: 'Regresar Dashboard', path: '../dashboard', state: this.paramFiltrosUrl},
    { title: 'Expedientes Web', path: '', disabled: true }
  ]

  get filtrarBusqueda() {
    return this._filtrarBusqueda;
  }
  set filtrarBusqueda (valor: string) {
    this._filtrarBusqueda = valor;
    this.listadoExpedientes = this.filtrarListado(valor);
    this.resultadobusqueda = this.listadoExpedientes.length ? false : true
    if (!valor) this.resultadobusqueda = false;
  }

  constructor(
    private authService : AuthService,
    private http: ReportesService,
    private modalService: BsModalService,
    private route: ActivatedRoute,
    private fn: Functions
  ) {
    this.permisos = this.authService.obtenerPermisos();
  }

  ngOnInit(): void {
    this.route.paramMap
    .pipe(map(() => window.history.state)).subscribe(res=> {
      if (res.idTipoExpediente) {
        this.paramFiltrosUrl.idTipoExpediente = res.idTipoExpediente
        this.paramFiltrosUrl.fechaInicial = res.fechaInicial;
        this.paramFiltrosUrl.fechaFinal = res.fechaFinal;
        this.paramFiltrosUrl.ejercicioFiscal = res.ejercicioFiscal;
        this.titulo = res.titulo;
        this.obtenerInformacionTablero(this.paramFiltrosUrl, false);
      }
    })
    this.obtenerTiposExpedientes();
  }


  public obtenerTiposExpedientes() : void {
    this.http.ObtenerTipoExpedientes().subscribe({
      next: (data) => {
        this.listadoTiposExpedientes = [...data];
      },
      error : (error) => { Notify.failure(error.message); }
    })
  }

  filtrarListado(busqueda : string) {
    if (!this.listadoExpedientesInicial.length || !this.filtrarBusqueda) {
      return this.listadoExpedientesInicial;
    } else {this.page = 1;
      return this.listadoExpedientesInicial.filter((option) => {
        return option.DESCRIPCION.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase()) ||
        option.ID_TIPO_EXPEDIENTE?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())||
        option.NO_EXPEDIENTE_MINFIN?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())||
        option.TIEMPO_TRANSCURRIDO?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())||
        option.USUARIO_ASIGNADO?.toString().trim().toLowerCase().includes(busqueda.trim().toLowerCase())
      })
    }
  }

  filterItems(item:any): any[] {
    let objeto = {...item}
    delete objeto.NO_EXPEDIENTE;
    delete objeto.NO_EXPEDIENTE_MINFIN;
    delete objeto.ID_TIPO_EXPEDIENTE;
    delete objeto.DESCRIPCION;
    delete objeto.USUARIO_ASIGNADO;
    delete objeto.TIEMPO_TRANSCURRIDO;
    delete objeto.EXTENSION;
    delete objeto.ID_UNIDAD_ADMINISTRATIVA;
    const propertyNames = Object.values(objeto);
    return propertyNames
  }

  public abrirModalExpedientes(fase : any) : void {
    const initialState: ModalOptions = {
      initialState: {
        list: {
          noExpediente: fase.NO_EXPEDIENTE,
          expedienteGlobal: fase.NO_EXPEDIENTE_GLOBAL,
          idTipoExpediente: fase.ID_TIPO_EXPEDIENTE,
        },
        titulo: fase.DESCRIPCION
      },
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-xl modal-dialog-centered'
    };

    this.bsModalRef = this.modalService.show(ModalExpedientesInfoComponent,  initialState);
    this.bsModalRef.content.closeBtnName = 'Cerrar vista';
  }

  abrirFiltrosAvanzados() {
    let nuevosHeaders = [...this.headers];
    // Se elimina el primer objeto del array que es el noExpediente
    nuevosHeaders.shift();
    let modelo = {...this.paramFiltrosUrl }
    modelo.fechaInicial = modelo.fechaInicial ? this.fn.transformarFecha(modelo.fechaInicial) : undefined;
    modelo.fechaFinal = modelo.fechaFinal ? this.fn.transformarFecha(modelo.fechaFinal) : undefined;


    const initialState: ModalOptions = {
      initialState: {
        title: "Filtros Avanzados",
        listaColumnas: nuevosHeaders,
        listaTiposExpedientes: this.listadoTiposExpedientes,
        formularioModelo: modelo
      },
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-lg modal-dialog-centered'
    };

    this.bsModalRef = this.modalService.show(ModalFiltrosAvanzadosComponent,  initialState);
    this.bsModalRef.content.closeBtnName = 'Cerrar vista';
    this.bsModalRef.content.filtrosSeleccionados.subscribe((filtros: IFiltrosAvanzadosPorTipoExpediente) => {
      if (filtros) {
        this.paramFiltrosUrl.fechaInicial = filtros.fechaInicial;
        this.paramFiltrosUrl.fechaFinal = filtros.fechaFinal;
        this.paramFiltrosUrl.ejercicioFiscal = filtros.ejercicioFiscal;
        this.paramFiltrosUrl.idTipoExpediente = filtros.idTipoExpediente;
        this.obtenerInformacionTablero(filtros, true);
      }
    })
  }


  obtenerInformacionTablero(filtros: IFiltrosAvanzadosPorTipoExpediente, cambiarTitulo: boolean) {
    this.http.obtenerExpedientesPorTipo(filtros).subscribe({
      next: (data) => {
        if(data.estado === EstadosHttp.success) {
          if (cambiarTitulo) {
            this.titulo = filtros.descripcionTipoExpediente ?? '';
          }
          this.bsModalRef?.hide();
          this.headers = data.resultado.header;
          setTimeout(() => {
            this.obtenerAnchoColumnas();
          }, 500);

          console.log("resultado de la solicitud:", data);

          this.listadoExpedientesInicial = [...data.resultado.expedientes]
          this.listadoExpedientes = [...data.resultado.expedientes]
        }
      },
      error : (error) => {
        Notify.failure(error.message);
      }
    })
  }

  obtenerAnchoColumnas() {
    let sizeObject = this.headers.length;
    let thead = document.querySelectorAll(".th-core");
    if (sizeObject < 10) {
      let widthScreen = 100
      let valWidth = widthScreen / sizeObject;
      let roundWidth =  valWidth.toFixed(2) + '%';
      thead.forEach((th:any) => {
        th.width = roundWidth;
      });
    } else {
      thead.forEach((th:any) => {
        th.style.maxWidth = '150px'
        th.style.minWidth= '150px'
      });
    }
  }

  public girarEncabezados(): void {
    let thead = document.querySelectorAll(".th-core");
    thead.forEach((th:any) => {
      if (!th.classList.contains('th-exp')) {
        if(th.classList.contains('horizontal')) {
          th.classList.remove('horizontal');
          th.classList.add('vertical');
          th.style.minWidth = '100px'
          th.style.maxWidth = '100px'
          th.style.height = '200px'
        } else {
          th.classList.remove('vertical');
          th.classList.add('horizontal');
          th.style.height = 'auto'
          th.style.maxWidth = '150px'
          th.style.minWidth= '150px'
        }
      }
    });
  }
}
