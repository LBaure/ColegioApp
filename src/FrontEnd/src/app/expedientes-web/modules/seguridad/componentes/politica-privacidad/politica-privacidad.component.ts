import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { UsuarioSSOModelo } from 'src/app/interfaces/usuario';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { ICamposHtml, IPoliticaPrivacidad, IPoliticaPrivacidadUsuario, IPoliticaUsuario } from 'src/app/expedientes-web/interfaces/seguridad/politica-privacidad';
import { IUsuario } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { AuthService } from 'src/app/utils/auth.service';
import { StartupConfigurationService } from 'src/app/utils/startup-configuration.service';
import Swal from 'sweetalert2';
import { SeguridadService } from '../../seguridad.service';
import { Notify } from 'notiflix';
import { ValidarCredencialesComponent } from 'src/app/expedientes-web/components/validar-credenciales/validar-credenciales.component';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { encode } from 'src/app/utils/functions/encode';
import { localStorageCore } from 'src/app/utils/functions/localStorageCore';

@Component({
  selector: 'app-politica-privacidad',
  templateUrl: './politica-privacidad.component.html',
  styleUrls: ['./politica-privacidad.component.css']
})
export class PoliticaPrivacidadComponent implements OnInit {
  loading : boolean = false;
  lockSaveButton: boolean = true;
  @Input() usuarioModelo : IUsuario = {} as IUsuario;
  public dobleFactorActivado: boolean = false;

  public itemsPrivacyPolicy : IPoliticaPrivacidad[] = [];
  public setCheck = new Array(2).fill(false);
  public FormPoliticaPrivacidad: FormGroup = {} as FormGroup;
  public etiquetaInput: string = "";
  public configCampos: ICamposHtml = {} as ICamposHtml;
  bsModalChangePinPass: BsModalRef | undefined;
  mq: SCREEN_SIZE = SCREEN_SIZE.LG;
  paso1: boolean = true;
  paso2: boolean = false;
  paso3: boolean = false;
  public itemsAttemps = new Array(1,2,3,4,5)
  public twoFactorAuth: boolean = false;
  private userLogin : UsuarioSSOModelo;

  constructor(
    private http : SeguridadService,
    private fb: FormBuilder,
    private modalService: BsModalService,
    private config : StartupConfigurationService,
    private auth : AuthService,
    private local: localStorageCore
  ) {
    this.FormPoliticaPrivacidad = this.obtenerControlesFormulario();
    this.userLogin = this.auth.obtenerUsuario();
  }

  ngOnInit(): void {
    this.obtenerPoliticasPrivacidad();
  }


  ngOnChanges(changes: SimpleChanges): void {
    this.intentosLogin?.setValue(this.usuarioModelo.intentosLogin);
    this.dobleFactorActivado = this.usuarioModelo.dobleFactorAuth;
  }

  private obtenerControlesFormulario(): FormGroup {
    return this.fb.group({
      idPolitica: [0, Validators.required],
      valor: [null, Validators.required],
      intentosLogin: [1, Validators.required],
      confirmarValor:[null]
    })
  }

  obtenerPoliticasPrivacidad() {
    this.http.obtenerPoliticasPrivacidad().subscribe({
      next: (data: IPoliticaPrivacidad[]) => {
        this.itemsPrivacyPolicy = [...data]
        this.setCheck = new Array(this.itemsPrivacyPolicy.length).fill(false)
      },
      error: (error) => {
        this.loading = false;
        Notify .failure(error.message, {
          position: 'right-bottom',
          fontSize: 'var(--bs-body-font-size)',
          clickToClose: true,
        });
      }
    })
  }

  /**
   * handleOnChange: obtiene las posiciones marcadas para el combo de items politica de privacidad
   * valida el formulario, agrega y quita validaciones.
   * @param position index del listado del array
   * @param element elemento o objeto del listado del array
   */
  administrarCambioPolitica = (position : number, element : IPoliticaPrivacidad) => {
    this.etiquetaInput = element.etiqueta;
    this.configCampos = {...element as ICamposHtml }

    this.FormPoliticaPrivacidad.get('valor')?.setValidators([
      Validators.required,
      Validators.pattern(element.pattern),
      Validators.minLength(element.minlength)
    ]);
    this.FormPoliticaPrivacidad.get('confirmarValor')?.setValidators([
      Validators.required,
      Validators.pattern(element.pattern),
      Validators.minLength(element.minlength)
    ]);
    this.FormPoliticaPrivacidad.get('intentosLogin')?.setValidators([
      Validators.required,
      Validators.minLength(1)
    ])

    this.FormPoliticaPrivacidad.updateValueAndValidity();
    /**
     * item son los valores true or false que trae cada elemento del array
     * index de cada elemento del array
     */
    const updatedCheckedState = this.setCheck.map((item,index) => {
      if (index === position) {
        if (item === true ){
          this.idPolitica?.setValue(0);
          this.etiquetaInput = '';
          this.valor?.clearValidators();
          this.valor?.updateValueAndValidity();
          return false
        } else {
          this.idPolitica?.setValue(element.idPolitica);
          this.pasoCompletado(3);
          return true
        }
      } else {
        return false
      }
    })
    this.setCheck = updatedCheckedState;
  };


  guardarPoliticaPrivacidad() {
    if (!this.FormPoliticaPrivacidad.valid) {
      Object.values(this.FormPoliticaPrivacidad.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    if (this.validarCoincidencia()) return;

    this.loading = true;
    const solicitud: IPoliticaUsuario = {
      nitUsuario: this.userLogin.nit,
      idPolitica : this.idPolitica?.value,
      valor: encode(this.valor?.value),
      intentosLogin: this.intentosLogin?.value
    }

    this.http.insertarPoliticaUsuario(solicitud).subscribe({
      next: (data: IRespuestaHttp) => { this.administrarRespuesta(data); },
      error: (error) => {
        Notify.failure(error.message);
        this.loading = false;
      },
      complete: () => {this.loading = false;},
    })
  }

  getMax(evt:any, val:number) {
    const txt =  evt.target.value
    if (txt.length == val) {
      return false;
    }
    return true
  }


  pasoCompletado(paso:number) {
    this.paso1 = false;
    this.paso2 = false;
    this.paso3 = false;
    switch (paso) {
      case 2:
        this.paso2 = true;
        break;
      case 3:
        this.paso3 = true;
        break;
      default:
        this.paso1 = true;
        break;
    }
  }

  inhabilitarDobleFactor() {
    if (this.usuarioModelo.dobleFactorObligatorio) {
      Swal.fire({
        text: 'No tienes permiso para realizar esta acción',
        icon: "warning",
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Aceptar'
      });
      return;
    }

    // OBTIENE LA CONFIGURACION DEL METODO DE AUTENTICACION DE DOS PASOS QUE EL USUARIO TIENE ACTIVO

    // loginInicial
    var esPrimerInicioSesion = this.local.getItem("loginInicial");
    const metodoEjecutar = (esPrimerInicioSesion) ? 'obtenerConfiguracionDobleFactor' : 'obtenerConfiguracionDobleFactorUsuario'

   this.http[metodoEjecutar]().subscribe({
      next: (datos: IRespuestaHttp) => {
        if (datos.estado === EstadosHttp.success) {
          this.abrirVentanaDobleFactor(datos.resultado)
        } else {
          this.administrarRespuesta(datos)
        }
      },
      error: (error) => { Notify.failure(error.message); },
      complete: () => { this.loading = false; }
    })
  }



















  private administrarRespuesta(resultado:IRespuestaHttp) : void {
    if(resultado.estado === EstadosHttp.success) {
      this.config.logout();
    }

    Swal.fire({
      html: resultado.mensaje,
      title: resultado.titulo,
      icon: resultado.icono
    })


    // Notify[resultado.estado as keyof typeof Notify](resultado.mensaje)
  }




  abrirVentanaDobleFactor(datos: IPoliticaPrivacidad) {
    const initialState = {
      descripcion: "Titulo",
      input: datos
    }

    this.bsModalChangePinPass = this.modalService.show(ValidarCredencialesComponent, {
      initialState,
      keyboard: true,
      backdrop: "static"
    })

    this.bsModalChangePinPass.content.EventoGuardar.subscribe((model:string) => {

      this.http.eliminarDobleFactor(model).subscribe({
        next: (datos: IRespuestaHttp) => {
          if (datos.estado === EstadosHttp.success) {
            this.bsModalChangePinPass?.hide();
            this.administrarRespuesta(datos);
          } else {
            this.administrarRespuesta(datos)
          }
        },
        error: (error) => {
          this.bsModalChangePinPass?.hide();
          Notify.failure(error.message);
        },
        complete: () => { this.loading = false; }
      })



    })

  }

  validarMetodoAutenticacion() {
    if (!this.valor?.value) return
    if (!this.confirmarValor?.value) return
    if (this.valor?.value !== this.confirmarValor?.value) {
      Swal.fire({
        text: 'Las contraseñas ingresadas no coinciden',
        icon: "warning",
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Aceptar'
      });
    }
  }

  validarCoincidencia() : boolean {
    let activar : boolean = false;
    if (this.valor?.value !== this.confirmarValor?.value) {
      activar = true;
    }
    return activar;
  }

  cambiarIconoInput(inputId:any, inputIcon: any) {
    var input = document.getElementById(inputId as keyof HTMLElement)
    var span = document.getElementById(inputIcon)

    if (input && span) {
      if (input["type" as keyof HTMLElement] == "password") {
        input.setAttribute('type', 'text');
        span.innerHTML= "visibility"
      } else {
        input.setAttribute('type', 'password');
        span.innerHTML= "visibility_off"
      }
    }
  }

  reiniciarFormulario() {
    this.valor?.setValue(null);
    this.valor?.markAsUntouched();
    this.valor?.markAsPristine();
    this.confirmarValor?.setValue(null);
    this.confirmarValor?.markAsUntouched();
    this.confirmarValor?.markAsPristine();
    this.FormPoliticaPrivacidad?.clearAsyncValidators();
    this.FormPoliticaPrivacidad?.updateValueAndValidity();
    this.pasoCompletado(2);
  }


  public get idPolitica() {return this.FormPoliticaPrivacidad.get('idPolitica');}
  public get valor() {return this.FormPoliticaPrivacidad.get('valor');}
  public get confirmarValor() {return this.FormPoliticaPrivacidad.get('confirmarValor');}
  public get intentosLogin() {return this.FormPoliticaPrivacidad.get('intentosLogin');}

}
