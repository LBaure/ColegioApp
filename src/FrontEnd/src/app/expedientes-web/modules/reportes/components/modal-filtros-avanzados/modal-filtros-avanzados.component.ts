import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFullYear } from 'ngx-bootstrap/chronos';
import { BsDatepickerConfig, BsDatepickerViewMode } from 'ngx-bootstrap/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IFiltrosAvanzadosPorTipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/filtros-avanzados-por-tipo-expediente';
import { ITipoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { deLocale } from 'ngx-bootstrap/locale';
import { ReportesService } from '../../reportes.service';
import { Notify } from 'notiflix';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { IFase } from 'src/app/expedientes-web/interfaces/dashboard/fase';
import { ResizeService } from 'src/app/utils/resize.service';
import { Subscription } from 'rxjs';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
defineLocale('es', deLocale);

interface columnaTemporal {
  columna: IHeader,
  indice: number
}

interface IFaseTemporal {
  fase: IFase,
  indice: number
}
@Component({
  selector: 'app-modal-filtros-avanzados',
  templateUrl: './modal-filtros-avanzados.component.html',
  styleUrls: ['./modal-filtros-avanzados.component.css']
})

export class ModalFiltrosAvanzadosComponent implements OnInit, OnDestroy {
  private servicesPantalla$ : Subscription = new Subscription;
  mq? : SCREEN_SIZE;
  title?: string;
  closeBtnName?: string;
  listaColumnas: IHeader[] = [];
  listaTiposExpedientes: ITipoExpediente[] = [];
  formularioModelo: Partial<IFiltrosAvanzadosPorTipoExpediente> = {};
  mostrarColumnas: boolean = true;
  mostrarNoExpediente: boolean = true;
  filtrosSeleccionados = new EventEmitter<IFiltrosAvanzadosPorTipoExpediente>();

  listadoFasesDisponibles: IFase[] = []
  listadoFasesMostrar: IFase[] = []
  listadoEjercicios: number[] = [];
  faseTemporal: IFaseTemporal = {} as IFaseTemporal;
  faseTemporalMostrar: IFaseTemporal = {} as IFaseTemporal;

  formFilter : FormGroup = {} as FormGroup;

  datePickerValue: Date = new Date();
  minMode: BsDatepickerViewMode = 'year';
  bsConfig?: Partial<BsDatepickerConfig>;
  bsConfigEjercicio?: Partial<BsDatepickerConfig>;

  maxDate = new Date();
  minDate = new Date();

  noExpedienteObligatorio:boolean = false;
  fechaFinalObligatorio:boolean = false;
  errorFechaFinal:boolean = false;
  filtrosAvanzados: IFiltrosAvanzadosPorTipoExpediente = {} as IFiltrosAvanzadosPorTipoExpediente;
  mensajeErrorFechaFinal: string = '';
  filtroEjercicioFiscal: boolean = true;
  filtroRangoFechas: boolean = false;
  rangoFechasObligatorio: boolean = false;


  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private resizeService : ResizeService,
    private http: ReportesService
  ) {
    this.formFilter = this.getFormControls();
    this.bsConfigEjercicio = Object.assign({}, {
      dateInputFormat: 'YYYY',
      containerClass: 'theme-dark-blue',
      minMode : this.minMode
    })

  }
  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
    this.bsModalRef?.hide();
  }

  cerrar() {
    this.bsModalRef.hide();

  }

  ngOnInit() {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      this.mq = x;

      if (x === SCREEN_SIZE.XS) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      }
    })


    if (this.formularioModelo) {
      this.formFilter.patchValue(this.formularioModelo)
      if (this.formularioModelo.fechaInicial) {
        this.tipoFiltro?.setValue(false)
        let fechaInicial = this.formularioModelo.fechaInicial;
        this.minDate = new Date(fechaInicial);
        this.maxDate = this.agregarAniosBusqueda(new Date(fechaInicial));
      }
      if (this.formularioModelo.idTipoExpediente) {
        this.buscarFases();
      }

      let fechaInicialBandera = this.formularioModelo.fechaInicial ? new Date(this.formularioModelo.fechaInicial) : null;
      this.fechaInicial?.setValue(fechaInicialBandera);

      let fechaFinalBandera = this.formularioModelo.fechaFinal ? new Date(this.formularioModelo.fechaFinal) : null;
      this.fechaFinal?.setValue(fechaFinalBandera);

    }

    this.bsConfig = Object.assign({}, {
      dateInputFormat: 'DD/MM/YYYY',
      containerClass: 'theme-dark-blue',
      locale: 'es',
      invalidDate: "Fecha Invalida"
    });
  }

  getFormControls() :FormGroup  {
    const group: FormGroup = this.fb.group({
      idTipoExpediente: [null],
      ejercicio: [null],
      numeroExpediente: [null],
      fechaInicial: null,
      fechaFinal: [null],
      fasesMostrar: [null],
      tipoFiltro: [true],
      ejercicioFiscal: [null]
    })
    return group;
  }

  public establecerReglas(value: Date): void {
    if(!value) {
      this.ejercicio?.setValue(null);
      this.numeroExpediente?.clearValidators();
      this.numeroExpediente?.updateValueAndValidity();
      return;
    }
    let year = getFullYear(value);
    if (isNaN(year)) {
      this.ejercicio?.setValue(null);
      this.numeroExpediente?.clearValidators();
      this.numeroExpediente?.updateValueAndValidity();
      return;
    }
    this.ejercicio?.setValue(year);
    this.numeroExpediente?.setValidators(Validators.required);
    this.numeroExpediente?.updateValueAndValidity();
  }

  aplicarFiltros() {
    console.log("this.formulario", this.formFilter);

    if (!this.formFilter.valid) {
      Object.values(this.formFilter.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    let itemTipoExpediente = this.listaTiposExpedientes.filter(el => el.idTipoExpediente  === this.idTipoExpediente?.value);
    if (!itemTipoExpediente.length){
      return
    }

    let datosEnviar : IFiltrosAvanzadosPorTipoExpediente = {
      idTipoExpediente: this.idTipoExpediente?.value,
      descripcionTipoExpediente: itemTipoExpediente[0].descripcion,
      ejercicio: this.ejercicio?.value,
      noExpediente: this.numeroExpediente?.value,
      fechaInicial: this.fechaInicial?.value ? this.fechaInicial.value.toLocaleDateString('en-UK') : null,
      fechaFinal: this.fechaFinal?.value ? this.fechaFinal.value.toLocaleDateString('en-UK') : null,
      fasesMostrar: this.obtenerFases(),
      ejercicioFiscal: this.ejercicioFiscal?.value
    }
    this.filtrosSeleccionados.emit(datosEnviar);
  }

  obtenerFases(): string[] {
    let fases: any[] = [];
    this.listadoFasesMostrar.forEach(element => {
      fases.push(element.idFase)
    });
    return fases.sort();
  }

  establecerReglasFecha(event: any) {
    if (event) {
      this.fechaFinalObligatorio = true;
      this.fechaFinal?.setValidators(Validators.required)
    }
    else {
      this.fechaFinalObligatorio = false;
      this.fechaFinal?.clearValidators();
      this.fechaFinal?.updateValueAndValidity();
    }

    // 1. Se borra el dato del campo Hasta de las Fechas de Registro Inmueble
    this.fechaFinal?.setValue(null);
    // 2. Se asigna la fecha minima al datepicker -> el valor de la propiedad
    this.minDate = new Date(event);
    // 3. Se obtiene la fecha final con 1 año agregado
    if (event) {
      this.maxDate = this.agregarAniosBusqueda(event);
    }
  }

  agregarAniosBusqueda (event: any) : Date {
    //1. Se extrae por partes la fecha de la propiedad entrante.
    var fechaRegistro = new Date(event);
    var anioSelecionado = fechaRegistro.getFullYear();
    var mesSelecionado = fechaRegistro.getMonth();
    var diaSelecionado = fechaRegistro.getDate();
    // 2. retorna la nueva fecha final al Datepicker, sumandole dos años
    var fechaFinal = new Date(anioSelecionado + 1, mesSelecionado, diaSelecionado);
    fechaFinal.setDate(fechaFinal.getDate() - 1);
    return fechaFinal;
  }

  validarFechas(event:Date) {
    if (!event) return;
    if (!this.fechaInicial?.value) {
      Notify.info("Ingrese una fecha inicial antes de agregar una fecha final.")
      this.fechaFinal?.setValue(null)
      return;
    }
    let fechaFinalInput = new Date(event.toDateString())
    if (fechaFinalInput > this.maxDate) {
      this.fechaFinal?.setValue(null);
      if (this.fechaInicial?.value) {
        this.errorFechaFinal = true;
        this.mensajeErrorFechaFinal = 'Los rangos maximos de fechas, deben ser menores a un año';
        setTimeout(() => {
          this.errorFechaFinal = false;
        }, 5000);
      }
    }

    if (event < this.minDate) {
      this.fechaFinal?.setValue(null);
      if (this.fechaInicial?.value) {
        this.errorFechaFinal = true;
        this.mensajeErrorFechaFinal = 'La fecha final no puede ser menor a la fecha inicial.';
        setTimeout(() => {
          this.errorFechaFinal = false;
        }, 5000);
      }
    }
  }


  public buscarFases():void {
    this.http.obtenerFasesPorTipoExpediente(this.idTipoExpediente?.value).subscribe({
      next: (data) => {
        if (data.estado !== EstadosHttp.success) return;
        this.listadoFasesDisponibles = [...data.resultado.listaFases];
        this.listadoEjercicios = [...data.resultado.listaEjercicios];
        if(this.tipoFiltro?.value) {
          this.ejercicioFiscal?.setValue(this.listadoEjercicios[this.listadoEjercicios.length - 1])
        }
      },
      error: (error) => {
        Notify.failure(error.message);
      }
    })
  }


  /**
   * agregarColumnaTemporal
   */
  public agregarColumnaTemporal(fase: IFase, indice: number, dobleClick: boolean, agregar: boolean) {
    this.faseTemporal.fase = fase;
    this.faseTemporal.indice = indice;
    if (dobleClick) {
      this.agregarListadoFasesMostrar();
    }
    this.eliminarFocoListadoFasesMostrar();
  }


  public agregarListadoFasesMostrar(): void {
    if(JSON.stringify(this.faseTemporal) == '{}') return;
    this.listadoFasesMostrar.push(this.faseTemporal.fase);
    this.listadoFasesDisponibles.splice(this.faseTemporal.indice, 1);
    let nuevoIndice = this.faseTemporal.indice + 1;
    let seleccionarSiguiente = document.getElementById("columna" + nuevoIndice);
    seleccionarSiguiente?.classList.add('active');
    let nuevaFase = this.listadoFasesDisponibles[this.faseTemporal.indice];
    if (nuevaFase) {
      this.agregarColumnaTemporal(nuevaFase, this.faseTemporal.indice, false, true)
    } else{
      this.faseTemporal = Object.assign({}, null)
    }
  }


  private eliminarFocoListadoFasesMostrar() {
    if (!this.listadoFasesMostrar.length) return;
    this.listadoFasesMostrar.forEach((element,index) => {
      let filaActiva = document.getElementById("columna_"+ index);
      if (filaActiva?.classList.contains('active')) {
        filaActiva.classList.remove('active')
        this.faseTemporalMostrar = Object.assign({}, null)
      }
    });
  }

  public quitarListadoFasesMostrar(fase: IFase, indice: number, dobleClick: boolean) {
    this.faseTemporalMostrar.fase = fase;
    this.faseTemporalMostrar.indice = indice;
    if (dobleClick) {
      this.agregarListadoFasesDisponibles();
    }
    this.eliminarFocoListadoFasesDisponibles();
  }

  public agregarListadoFasesDisponibles(): void {
    if(JSON.stringify(this.faseTemporalMostrar) == '{}') return;
    this.listadoFasesDisponibles.push(this.faseTemporalMostrar.fase);
    this.listadoFasesMostrar.splice(this.faseTemporalMostrar.indice, 1);
    let nuevoIndice = this.faseTemporalMostrar.indice + 1;
    let seleccionarSiguiente = document.getElementById("columna_" + nuevoIndice);
    seleccionarSiguiente?.classList.add('active');
    let nuevaFase = this.listadoFasesMostrar[this.faseTemporalMostrar.indice];
    if (nuevaFase) {
      this.quitarListadoFasesMostrar(nuevaFase, this.faseTemporalMostrar.indice, false);
    } else {
      this.faseTemporalMostrar = Object.assign({}, null)
    }
  }

  private eliminarFocoListadoFasesDisponibles() {
    if (!this.listadoFasesDisponibles.length) return;
    this.listadoFasesDisponibles.forEach((element,index) => {
      let filaActiva = document.getElementById("columna"+ index);
      if (filaActiva?.classList.contains('active')) {
        filaActiva.classList.remove('active')
        this.faseTemporal = Object.assign({}, null)
      }
    });
  }

  /**
   * limpiarRangoFech()
   */
  public limpiarRangoFechas() {
    this.fechaInicial?.setValue(null);
    this.fechaFinal?.setValue(null);
    this.maxDate = new Date();
    this.minDate = new Date();

    this.fechaInicial?.clearValidators();
    this.fechaFinal?.clearValidators();
    this.fechaInicial?.updateValueAndValidity();
    this.fechaFinal?.updateValueAndValidity();

    this.ejercicioFiscal?.setValue(this.listadoEjercicios[this.listadoEjercicios.length - 1])
  }


  /**
   * validarRangoFechas
   */
  public validarRangoFechas(): void {
    this.ejercicioFiscal?.setValue(null);
    this.rangoFechasObligatorio = true;
    this.fechaInicial?.addValidators(Validators.required);
    this.fechaFinal?.addValidators(Validators.required);
    this.formFilter.updateValueAndValidity();
  }



  public get idTipoExpediente() {return this.formFilter.get('idTipoExpediente');}
  public get numeroExpediente() {return this.formFilter.get('numeroExpediente');}
  public get ejercicio() {return this.formFilter.get('ejercicio');}
  public get fechaInicial() {return this.formFilter.get('fechaInicial');}
  public get fechaFinal() {return this.formFilter.get('fechaFinal');}
  public get fasesMostrar() {return this.formFilter.get('fasesMostrar');}
  public get tipoFiltro() {return this.formFilter.get('tipoFiltro');}
  public get ejercicioFiscal() {return this.formFilter.get('ejercicioFiscal');}

}



















