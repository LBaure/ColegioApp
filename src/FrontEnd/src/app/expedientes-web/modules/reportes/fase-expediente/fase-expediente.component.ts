import { Component, OnDestroy, OnInit } from '@angular/core';
import { FaseInfo, ReportesService } from '../reportes.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ModalExpedientesInfoComponent } from '../components/modal-expedientes-info/modal-expedientes-info.component';
import { ModalTipoExpedientesComponent } from '../components/modal-tipo-expedientes/modal-tipo-expedientes.component';
import { Expediente, IExpedientesWorkFlow, IFasesConExpedientesWorkFlow, IFasesExpediente, ITipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';
import { Notify } from 'notiflix';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Subscription } from 'rxjs';
import { ModalFiltrarFasesComponent } from '../components/modal-filtrar-fases/modal-filtrar-fases.component';
import { ModalFiltrosAvanzadosComponent } from '../components/modal-filtros-avanzados/modal-filtros-avanzados.component';
import { IFiltrosAvanzadosPorTipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/filtros-avanzados-por-tipo-expediente';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
@Component({
  selector: 'app-fase-expediente',
  templateUrl: './fase-expediente.component.html',
  styleUrls: ['./fase-expediente.component.css']
})
export class FaseExpedienteComponent implements OnInit, OnDestroy {
  bsModalRef?: BsModalRef;
  bsModalFiltroAvanzado?: BsModalRef;
  bsModalRefTipoExpediente?: BsModalRef;
  renderizarDashboard: boolean = false;
  tipoExpediente: ITipoExpediente = {} as ITipoExpediente;
  faseExpediente: IFasesExpediente[] = [];
  listaFasesMostrar: IFasesExpediente[] = [];
  listadoTiposExpedientes: ITipoExpediente[] = [];
  paramFiltrosAvanzados: Partial<IFiltrosAvanzadosPorTipoExpediente> = {};

  // nuevo listado

  listadoExpedientesWorkFlow: IFasesConExpedientesWorkFlow[] = [];


  p_tipoExpediente: string | null= '';
  filtrarBusqueda: string = "";


  private routeSub: Subscription = new Subscription;

  /**
   *
   */
  constructor(
    private modalService: BsModalService,
    private http: ReportesService,
    private route: ActivatedRoute
  ) { }


  ngOnInit(): void {
    this.routeSub = this.route.paramMap.subscribe((params: ParamMap) => {
      this.p_tipoExpediente = params.get('tipo-expediente');
    });

    if( !this.p_tipoExpediente ) {
      this.filtrarTipoExpediente();
    } else {
      let filtros: IFiltrosAvanzadosPorTipoExpediente = {
        idTipoExpediente: this.p_tipoExpediente
      }
      this.obtenerInformacionTablero(filtros);
    }

    // this.filtrosAvanzados()

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




  // 1. Muestra los tipos de expedientes disponibles para el usuario.
  // 2. Al seleccionar un tipo de expediente, carga los expedientes de este tipo con todas sus fases.
  filtrarTipoExpediente() {
    let config = {
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-xl modal-dialog-centered'
    };
    this.bsModalRefTipoExpediente = this.modalService.show(ModalTipoExpedientesComponent,  config);
    this.bsModalRefTipoExpediente.content.EnviarTipoExpediente.subscribe( (tipoExpediente : ITipoExpediente) => {
      if (tipoExpediente) {
        this.tipoExpediente = tipoExpediente;
        let filtros: IFiltrosAvanzadosPorTipoExpediente = {
          idTipoExpediente: tipoExpediente.idTipoExpediente
        }
        this.obtenerInformacionTablero(filtros, this.bsModalRefTipoExpediente);
      }
    })
  }

  filtrarFasesExpediente() {
    let config = {
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-xl modal-dialog-centered',
    };

    const initialState: any = {
      listaFases: this.faseExpediente,
      listaFasesMostrar: this.listaFasesMostrar
    }


    this.bsModalRefTipoExpediente = this.modalService.show(ModalFiltrarFasesComponent,  { initialState, class: 'modal-lg modal-dialog-centered' });
    // this.bsModalRefTipoExpediente.content.EnviarTipoExpediente.subscribe( (tipoExpediente : ITipoExpediente) => {
    //   console.log("EnviarTipoExpediente", tipoExpediente);

    //   if (tipoExpediente) {
    //     this.tipoExpediente = tipoExpediente;
    //     this.obtenerExpedientes(tipoExpediente.idTipoExpediente);
    //   }
    // })
  }

  renderizarWorkFlow() {
    this.renderizarDashboard = true
    let final = this.listadoExpedientesWorkFlow.length;
    setTimeout(() => {
      let barra = document.getElementsByClassName("box-title")
      let h4 = barra[final-1]
      h4.classList.add('closed')
    }, 0);
  }

  abrirModalExpedientes(fase : IExpedientesWorkFlow) {
    const initialState: ModalOptions = {
      initialState: {
        list: {
          noExpediente: fase.noExpediente,
          expedienteGlobal: fase.noExpedienteMinfin,
          idTipoExpediente: fase.idTipoExpediente,
        },
        titulo: fase.descripcion
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
    // let nuevosHeaders = [...this.headers];
    // // Se elimina el primer objeto del array que es el noExpediente
    // nuevosHeaders.shift()

    this.paramFiltrosAvanzados.idTipoExpediente = this.tipoExpediente.idTipoExpediente;
    const initialState: ModalOptions = {
      initialState: {
        title: "Búsqueda de expedientes",
        listaColumnas: [],
        listaTiposExpedientes: this.listadoTiposExpedientes,
        formularioModelo: this.paramFiltrosAvanzados
      },
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-lg modal-dialog-centered'
    };

    this.bsModalFiltroAvanzado = this.modalService.show(ModalFiltrosAvanzadosComponent,  initialState);
    this.bsModalFiltroAvanzado.content.closeBtnName = 'Cerrar vista';
    this.bsModalFiltroAvanzado.content.filtrosSeleccionados.subscribe((filtros: IFiltrosAvanzadosPorTipoExpediente) => {
      this.obtenerInformacionTablero(filtros, this.bsModalFiltroAvanzado);
    })
  }


  obtenerInformacionTablero(filtros: IFiltrosAvanzadosPorTipoExpediente, modalRef?: BsModalRef) {

    this.http.obtenerExpedientesWorkFlow(filtros).subscribe({
      next: (data) => {
        if(data.estado === EstadosHttp.success) {
          this.listadoExpedientesWorkFlow = [...data.resultado];
          this.renderizarWorkFlow();
          if (modalRef) {
            modalRef?.hide();
          }
        }
      },
      error : (error) => {
        Notify.failure(error.message);
      }
    })
  }


  ngOnDestroy(): void {
    // Asegurarse de desuscribirse para evitar pérdida de memoria
    this.routeSub.unsubscribe();
  }
}
