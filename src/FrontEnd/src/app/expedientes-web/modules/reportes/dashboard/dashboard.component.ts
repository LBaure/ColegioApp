import { Estado } from './../../../../interfaces/usuario';
import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import Chart from 'chart.js/auto';
import { IBreadcrumb } from 'src/app/componentes-html/interfaces/models';
import { ReportesService } from '../reportes.service';
import { Notify } from 'notiflix';
import { IDatosDashboardModelo, IFaseExpedienteModelo, ITotalExpedienteFaseModelo, IUsuarioExpedienteModelo } from 'src/app/expedientes-web/interfaces/dashboard/total-expediente-fase';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ModalTipoExpedientesComponent } from '../components/modal-tipo-expedientes/modal-tipo-expedientes.component';
import { ITipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';
import { IFiltrosExpedientesPorTipo } from 'src/app/expedientes-web/interfaces/dashboard/filtros-expedientes-por-tipo';
import { IFiltrosAvanzadosPorTipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/filtros-avanzados-por-tipo-expediente';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { ModalFiltrosAvanzadosComponent } from '../components/modal-filtros-avanzados/modal-filtros-avanzados.component';
import { map } from 'rxjs';
import { Functions } from 'src/app/utils/functions/functions';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

  titulo: string = "";
  tituloGrafico: string = "";
  bsModalRef?: BsModalRef;
  headers: IHeader[] = [];
  datosDashboard: IDatosDashboardModelo = {} as IDatosDashboardModelo;
  listaTotalUsuariosTipoExpedientes: IUsuarioExpedienteModelo[] = [];
  listaTotalFasesTipoExpedientes: ITotalExpedienteFaseModelo = {} as ITotalExpedienteFaseModelo;
  graficoGenerado: boolean = false;

  bsModalRefTipoExpediente?: BsModalRef;
  tipoExpediente: ITipoExpediente = {} as ITipoExpediente;
  //fechaActualInicio: string = '';
  //fechaActualFin: string = '';
  ejercicioActual: number = 0;

  listadoTiposExpedientes: ITipoExpediente[] = [];
  listaParaGraficoDona: IFaseExpedienteModelo[] = [];
  paramFiltros: Partial<IFiltrosAvanzadosPorTipoExpediente> = {};



  listaEjeX = [];
  routes: IBreadcrumb[] = [
    {
      path: '',
      title: 'Dashboard'
    }
  ]
  chart: any;
  chartDonna: any;

  constructor(
    private router : Router,
    private http: ReportesService,
    private modalService: BsModalService,
    private activatedRoute: ActivatedRoute,
    private fn: Functions
  ) {

  }

  ngOnInit(): void {
    this.obtenerTiposExpedientes();
    this.activatedRoute.paramMap
    .pipe(map(() => window.history.state)).subscribe(res=> {
      if (res.idTipoExpediente) {
        //console.log("---------QUE TRAE ACTIVATEDROUTE: ----------", res);
        //console.log('LISTADO EXPEDIENTES ROUTE',this.listadoTiposExpedientes);
        this.http.ObtenerTipoExpedientes().subscribe({
          next: (data) => {
            this.listadoTiposExpedientes = [...data];
            this.tipoExpediente = this.listadoTiposExpedientes.find(e=> e.idTipoExpediente == res.idTipoExpediente)!;
            this.paramFiltros.idTipoExpediente = res.idTipoExpediente;
            this.paramFiltros.ejercicioFiscal = res.ejercicioFiscal;
            this.ejercicioActual = res.ejercicioFiscal;
            this.paramFiltros.fechaInicial = res.fechaInicial;
            this.paramFiltros.fechaFinal = res.fechaFinal;
            this.titulo = this.tipoExpediente.descripcion;
            //console.log(res.ejercicioFiscal)
            this.obtenerExpedientes(1,res.idTipoExpediente, res.fechaInicial, res.fechaFinal, res.ejercicioFiscal)
          },
          error : (error) => { Notify.failure(error.message); }
        })

      }
      else {

        this.seleccionarTipoExpedientes();


      }
    })




    this.tituloGrafico = "Exp. Generados vs. Finalizados"
  }

  obtenerTiposExpedientes() {
    this.http.ObtenerTipoExpedientes().subscribe({
      next: (data) => {
        //console.log("lista", data);

        this.listadoTiposExpedientes = [...data];
      },
      error : (error) => { Notify.failure(error.message); }
    })
  }



  seleccionarTipoExpedientes() {
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
        this.titulo = tipoExpediente.descripcion;
        this.paramFiltros.idTipoExpediente = tipoExpediente.idTipoExpediente;
        this.paramFiltros.fechaInicial = "";
        this.paramFiltros.fechaFinal = "";

        this.obtenerExpedientes(1, tipoExpediente.idTipoExpediente);


      }
    })
  }

  obtenerExpedientes(modalClose: number, tipoExpediente: string, fechaIni?: string, fechaFin?: string, ejeFiscal?: number): boolean {
    if(tipoExpediente == null){
      Notify.failure("No se han ingresado tipo de expediente a consultar");
      return false;
    }

    let filtros : IFiltrosExpedientesPorTipo = {
      idTipoExpediente: tipoExpediente,
      fechaInicial: fechaIni,
      fechaFinal: fechaFin,
      ejercicioFiscal: ejeFiscal
    }
    //console.log("ENTRO AL DASHBOARD");
    this.http.obtenerDatosDashboard(filtros).subscribe({
      next: (data) => {
        if(data.estado == EstadosHttp.warning){
          Notify.warning("El tipo de expediente seleccionado no contiene expedientes para mostrar");
          return false;
        }else if(data.estado == EstadosHttp.info){
          Notify.warning(data.mensaje);
        }
        this.tituloGrafico = "Exp. Generados vs. Finalizados del " + data.resultado.fechaInicial + " al " + data.resultado.fechaFinal;
        this.listaTotalUsuariosTipoExpedientes = data.resultado.cantidadExpedientesPorUsuarios;
        if (data.resultado.cantidadExpedientesPorFase.fases.length) {
          data.resultado.cantidadExpedientesPorFase.fases.map((el: { descripcion: string; }) => {
            el.descripcion = el.descripcion.toUpperCase()
          })
        }
        this.listaParaGraficoDona = JSON.parse(JSON.stringify(data.resultado.cantidadExpedientesPorFase.fases));
        this.listaParaGraficoDona.forEach( e => {
          e.descripcion = e.descripcion + "\n" + e.idFase;
        })
        let jsonFases = JSON.stringify(data.resultado.cantidadExpedientesPorFase);
        this.listaTotalFasesTipoExpedientes = JSON.parse(jsonFases);
        this.listaTotalFasesTipoExpedientes.fechaInicial = fechaIni;
        this.listaTotalFasesTipoExpedientes.fechaFinal = fechaFin;
        this.listaTotalFasesTipoExpedientes.ejercicioFiscal = ejeFiscal;
        this.listaTotalFasesTipoExpedientes.titulo = this.titulo;

        //console.log(data, "RESULTADOOOOOOO");

        //this.bsModalRefTipoExpediente?.hide();
        if(modalClose == 1){
          this.bsModalRefTipoExpediente?.hide();
        }
        else{
          this.bsModalRef?.hide();
        }

        if(this.graficoGenerado){
          this.chartDonna.destroy();
          this.chart.destroy();
        }

        this.crearGraficoDona(this.listaParaGraficoDona);
        this.crearGraficoGeneradosVsFinalizados(data.resultado.ejeX, data.resultado.ejeYGenerados, data.resultado.ejeYFinalizados);
        this.graficoGenerado=true;
        return true;
      },
      error : (error) => {
        Notify.failure(error.message);
        return false;
      }


    })
    return false;

  }



  crearGraficoDona(fases : IFaseExpedienteModelo[]) {
    let etiquetas : string[] = [];
    fases.forEach(fase => {
      etiquetas.push(fase.descripcion.toUpperCase());
    });

    const data = {
      labels: etiquetas,
      datasets: [{
        label: 'Total de expedientes',
        data: fases
      }]
    };
    this.chartDonna = new Chart("MyChartDoughnut", {
      type: "doughnut",
      data: data,
      options: {
        parsing: {
          key: 'total'
        },
        plugins: {
          legend: {
            display: false
          },
          title: {
            display: true,

            font: {
              size: 18
            }
          }
        }
      }
    });


    setTimeout(() => {
      let listaColores: any = this.chartDonna.config._config.data.datasets[0].backgroundColor;
      for (let i = 0; i < this.listaTotalFasesTipoExpedientes.fases.length; i++) {
        if (this.listaTotalFasesTipoExpedientes.fases[i].total > 0){
          this.listaTotalFasesTipoExpedientes.fases[i].color = listaColores[i]
        }
      }
    }, 0);

  }

  crearGraficoGeneradosVsFinalizados(listaEjeXGenerados: string[], datosEjeYGenerados: number[], datosEjeYFinalizados: number[]){
    this.chart = new Chart("MyChart", {
      type: "line",
      data: {

        labels: listaEjeXGenerados,
        datasets: [

        {
          label: 'Cant. expedientes generados',
          data: datosEjeYGenerados,
          borderColor: "green",
          fill: false
        },
        {
          label: 'Cant. expedientes finalizados',
          data: datosEjeYFinalizados,
          borderColor: "red",
          fill: false
        }
      ]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true,


          },
          title: {
            display: true

          }
        }
      }
    });
  }



  abrirFiltrosAvanzados() {
    let nuevosHeaders = [...this.headers];
    // Se elimina el primer objeto del array que es el noExpediente
    nuevosHeaders.shift()
    let modelo = {...this.paramFiltros }
    modelo.ejercicioFiscal = this.ejercicioActual;
    modelo.fechaInicial = modelo.fechaInicial ? this.fn.transformarFecha(modelo.fechaInicial) : undefined;
    modelo.fechaFinal = modelo.fechaFinal ? this.fn.transformarFecha(modelo.fechaFinal) : undefined;

    const initialState: ModalOptions = {
      initialState: {
        title: "Filtros Avanzados",
        listaColumnas: nuevosHeaders,
        listaTiposExpedientes: this.listadoTiposExpedientes,
        mostrarColumnas: false,
        mostrarNoExpediente: false,
        formularioModelo: modelo
      },
      backdrop: true,
      keyboard: false,
      ignoreBackdropClick: true,
      class: 'modal-lg modal-dialog-centered'
    };
    //console.log('FILTROS INGRESO MODAL', modelo);

    this.bsModalRef = this.modalService.show(ModalFiltrosAvanzadosComponent,  initialState);
    this.bsModalRef.content.closeBtnName = 'Cerrar vista';
    this.bsModalRef.content.filtrosSeleccionados.subscribe((filtros: IFiltrosAvanzadosPorTipoExpediente) =>{
      //console.log(filtros);
      if (filtros) {
        if(filtros.idTipoExpediente == null){
          Notify.failure("No se han ingresado tipo de expediente a consultar");
          return;
        }

        if(filtros.fechaInicial == null && filtros.fechaFinal != null){
          Notify.failure("No se han ingresado correctamente los rangos de fecha para el tipo de expediente a consultar");
          return;
       }

        this.ejercicioActual = filtros.ejercicioFiscal!;
        this.tipoExpediente = this.listadoTiposExpedientes.find(e=> e.idTipoExpediente == filtros.idTipoExpediente)!;
        this.paramFiltros.idTipoExpediente = this.tipoExpediente.idTipoExpediente;
        this.paramFiltros.fechaInicial = filtros.fechaInicial;
        this.paramFiltros.fechaFinal = filtros.fechaFinal;
        this.titulo = this.tipoExpediente.descripcion;

        this.obtenerExpedientes(2, filtros.idTipoExpediente!, filtros.fechaInicial!, filtros.fechaFinal!, filtros.ejercicioFiscal);
      }
    })
  }

}
