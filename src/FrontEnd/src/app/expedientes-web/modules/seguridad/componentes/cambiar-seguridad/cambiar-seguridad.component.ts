import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsuarioSSOModelo } from 'src/app/interfaces/usuario';
import { ICambioCredenciales, ICamposHtml, IPoliticaPrivacidad } from 'src/app/expedientes-web/interfaces/seguridad/politica-privacidad';
import { ICambioPinContrasenia, IUsuario } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { AuthService } from 'src/app/utils/auth.service';
import { SeguridadService } from '../../seguridad.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { StartupConfigurationService } from 'src/app/utils/startup-configuration.service';
import Swal from 'sweetalert2';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { Notify } from 'notiflix';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ValidarCredencialesComponent } from 'src/app/expedientes-web/components/validar-credenciales/validar-credenciales.component';
import { encode } from 'src/app/utils/functions/encode';
import { localStorageCore } from 'src/app/utils/functions/localStorageCore';

@Component({
  selector: 'app-cambiar-seguridad',
  templateUrl: './cambiar-seguridad.component.html',
  styleUrls: ['./cambiar-seguridad.component.css']
})
export class CambiarSeguridadComponent implements OnInit, OnChanges{
  @Input() usuarioModelo : IUsuario = {} as IUsuario;


  public loading: boolean = false;
  public credencialObligatoria: boolean = false;
  public formCambiarPinContrasenia : FormGroup = {} as FormGroup;
  public configInput : IPoliticaPrivacidad = {} as IPoliticaPrivacidad;
  public listaIntentos =  [1,2,3,4,5];
  private bsModalValidarCredenciales: BsModalRef | undefined;

  constructor(
    private fb : FormBuilder,
    private http : SeguridadService,
    private modalService: BsModalService,
    private config : StartupConfigurationService,
    private local: localStorageCore
  ) {
    this.formCambiarPinContrasenia = this.obtenerControlesFormulario();
  }

  private obtenerControlesFormulario() : FormGroup {
    return this.fb.group({
      valorNuevo: [null],
      confirmarValorNuevo: [null],
      intentosLogin: [1, Validators.required]
    })
  }

  obtenerReglas(){
    if (this.valorNuevo?.value) {
      this.credencialObligatoria = true;
      this.valorNuevo.setValidators([
        Validators.required,
        Validators.pattern(this.configInput.pattern),
        Validators.minLength(this.configInput.minlength)
      ]);
      this.confirmarValorNuevo?.setValidators([
        Validators.required,
        Validators.pattern(this.configInput.pattern),
        Validators.minLength(this.configInput.minlength)
      ]);
      this.valorNuevo.updateValueAndValidity();
      this.confirmarValorNuevo?.updateValueAndValidity();
    } else {
      this.credencialObligatoria = false;
      this.valorNuevo?.clearValidators();
      this.confirmarValorNuevo?.clearValidators();
      this.valorNuevo?.updateValueAndValidity();
      this.confirmarValorNuevo?.updateValueAndValidity();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.usuarioModelo.nitUsuario){
      this.intentosLogin?.setValue(this.usuarioModelo.intentosLogin);
      this.obtenerPoliticaPrivacidad();
    }
  }
  obtenerPoliticaPrivacidad () {
    var esPrimerInicioSesion = this.local.getItem("loginInicial");
    const metodoEjecutar = (esPrimerInicioSesion) ? 'obtenerConfiguracionDobleFactor' : 'obtenerConfiguracionDobleFactorUsuario'
    // OBTIENE LA CONFIGURACION DEL METODO DE AUTENTICACION DE DOS PASOS QUE EL USUARIO TIENE ACTIVO
    this.http[metodoEjecutar]()
    .subscribe({
      next: (datos: IRespuestaHttp) => {
        if (datos.estado === EstadosHttp.success) {
          let politica : IPoliticaPrivacidad = datos.resultado;
          if (politica){
            this.configInput = {...politica};
          }
        }
      },
      error: (error) => { Notify.failure(error.message); },
      complete: () => { this.loading = false; }
    })
  }

  ngOnInit(): void {
  }

  cambiarIcono(inputId:any, inputIcon: any) {
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


  cancelarCambios() : void {
    this.credencialObligatoria = false;
    this.formCambiarPinContrasenia.reset({
      valorNuevo: '',
      confirmarValorNuevo: '',
      intentosLogin: this.usuarioModelo.intentosLogin
    })
    this.valorNuevo?.clearValidators();
    this.confirmarValorNuevo?.clearValidators();
    this.valorNuevo?.updateValueAndValidity();
    this.confirmarValorNuevo?.updateValueAndValidity();
  }

  validarCoincidencia() : boolean {
    let activar : boolean = false;
    if (this.valorNuevo?.value !== this.confirmarValorNuevo?.value) {
      activar = true;
    }
    return activar;
  }
  guardarNuevasCredenciales() : void {
    if (!this.formCambiarPinContrasenia.valid) {
      Object.values(this.formCambiarPinContrasenia.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    if (!this.valorNuevo?.value && (this.intentosLogin?.value === this.usuarioModelo.intentosLogin)){
      Notify.info("No existen cambios para guardar.")
      return;
    }

    if (this.validarCoincidencia()) return;

    let nuevoValor: string = this.valorNuevo?.value ? encode(this.valorNuevo?.value) : null

    // Estado inicial del componente.
    const initialState = {
      descripcion: "Titulo",
      input: this.configInput
    }

    this.bsModalValidarCredenciales = this.modalService.show(ValidarCredencialesComponent, {
      initialState,
      keyboard: true,
      backdrop: "static"
    })

    this.bsModalValidarCredenciales.content.EventoGuardar.subscribe((contrasenia:string) => {
      let datosEnviar: ICambioCredenciales = {
        intentosLogin: this.intentosLogin?.value,
        valorActual: contrasenia,
        idPolitica: nuevoValor ? this.configInput.idPolitica : null,
        valorNuevo: nuevoValor
      }

      this.http.actualizarPoliticaUsuario(datosEnviar).subscribe({
        next: (resultado) => { this.administrarRespuesta(resultado, nuevoValor); },
        error: (error) => { Notify.failure(error.message); }
      })
    })
  }



  private administrarRespuesta(resultado:IRespuestaHttp, valorNuevo: string) : void {
    if(resultado.estado === EstadosHttp.success) {
      if (valorNuevo) {
        this.config.logout();
      } else {
        this.usuarioModelo.intentosLogin = this.intentosLogin?.value
      }
      this.bsModalValidarCredenciales?.hide();
    }

    Swal.fire({
      html: resultado.mensaje,
      title: resultado.titulo,
      icon: resultado.icono
    })


    // Notify[resultado.estado as keyof typeof Notify](resultado.mensaje)
  }




  getMax(evt:any, val:number) {
    const txt =  evt.target.value
    if (txt.length == val) {
      return false;
    }
    return true
  }


  get valorNuevo() { return this.formCambiarPinContrasenia.get('valorNuevo'); }
  get confirmarValorNuevo() { return this.formCambiarPinContrasenia.get('confirmarValorNuevo'); }
  get intentosLogin() { return this.formCambiarPinContrasenia.get('intentosLogin'); }



}
