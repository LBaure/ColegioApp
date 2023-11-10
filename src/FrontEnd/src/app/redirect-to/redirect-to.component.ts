import { Component, Injector, OnInit } from '@angular/core';
import { StartupConfigurationService } from '../utils/startup-configuration.service';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { TwoFactorAuthComponent } from '../expedientes-web/components/two-factor-auth/two-factor-auth.component';
import { ConfigUserModel } from '../expedientes-web/interfaces/usuario';
import { configInput } from '../expedientes-web/interfaces/ConfigInput';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { ICredencialesUsuario } from '../expedientes-web/interfaces/Login';
import { SeguridadService } from '../expedientes-web/modules/seguridad/seguridad.service';
import { IRespuestaHttp } from '../expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from '../expedientes-web/constants/estado-http';
import { encode } from '../utils/functions/encode';
import { Notify } from 'notiflix';
import { localStorageCore } from '../utils/functions/localStorageCore';
import { AuthService } from '../utils/auth.service';

@Component({
  selector: 'app-redirect-to',
  templateUrl: './redirect-to.component.html',
  styleUrls: ['./redirect-to.component.css']
})
export class RedirectToComponent implements OnInit {
  private bsModalChangePinPass: BsModalRef | undefined;

  constructor(
    private config : StartupConfigurationService,
    private injector: Injector,
    private modalService: BsModalService,
    private http : SeguridadService,
    private router: Router,
    private local: localStorageCore,
    private auth: AuthService
  ) { }

  ngOnInit(): void {
    console.log("this.auth.isAuthenticated()", this.auth.isAuthenticated());


    if (!this.auth.isAuthenticated()) {
      this.auth.navigate('login')
    }


    this.config.obtenerSesionActual().then((usuario) => {

      if (!usuario) { this.injector.get(Router).navigate(['public']); }
      switch (true) {
        case usuario.dobleFactorObligatorio && usuario.idPolitica < 1:
          this.validarCuenta();
          break;
        case usuario.dobleFactorAuth === false:
          // DOBLE FACTOR DE AUTENTICACION ESTA INHABILITADO
          const solicitud : ICredencialesUsuario = {
            nitUsuario: this.config.usuario.nit,
            dobleFactor: usuario.dobleFactorAuth
          }
          this.iniciarSesionConDobleFactor(solicitud)
          break;
        case usuario.sesionActiva:
         this.auth.navigate('/ew/dashboard')
          break;
        default:
          this.openModalTwoFactorAuthentication(usuario);
          break;
      }
    },
    (error) => {
      if (error.estado){
        Notify.failure(error.mensaje);
      } else {
        Notify[error.status as keyof typeof Notify](error.message);
      }
      this.injector.get(Router).navigate(['public']);
    });
  }


  validarCuenta() {
    Swal.fire({
      icon: "warning",
      text: 'Para activar su usuario por favor, agregue un metodo Verificación en dos pasos, en la pestaña Politica de privacidad',
      confirmButtonText: 'Aceptar',
      allowOutsideClick: false,
    }).then((result) => {
      if (result.isConfirmed) {
        this.local.setItem("loginInicial", 'true');
        debugger
        this.router.navigate(['/ew/seguridad/mi-perfil'])
      } else {
        this.injector.get(Router).navigate(['public']);
      }
    })
  }

  openModalTwoFactorAuthentication(data:ConfigUserModel) {
    const configInput : configInput = {
      etiqueta: data.etiqueta,
      pattern: data.pattern,
      required: true,
      messagePattern: data.messagePattern,
      messageMinLength: data.messageMinlength,
      messageRequired: data.messageRequired,
      maxLength: data.maxLength,
      minLength: data.minLength,
      mask: data.mask,
      class: data.class,
      descripcion: data.descripcion,
      descriptionLogin: data.descriptionLogin
    }

    const initialState = {
      titleModal: `Verificacion de dos pasos`,
      btnCancel: false,
      configInput: configInput
    }

    this.bsModalChangePinPass = this.modalService.show(TwoFactorAuthComponent, {
      initialState,
      keyboard: false,
      backdrop: false,
      ignoreBackdropClick: true
    })

    this.bsModalChangePinPass.content.EventModel.subscribe((model:string) => {
      const reqLogin : ICredencialesUsuario = {
        nitUsuario: this.config.usuario.nit,
        valor: model,
        dobleFactor: data.dobleFactorAuth
      }
      this.iniciarSesionConDobleFactor(reqLogin);
    })
  }

  iniciarSesionConDobleFactor(credencialUsuario: ICredencialesUsuario) {
    this.http.login(credencialUsuario).subscribe({
      next: (data:IRespuestaHttp) => {
        if(data.estado === EstadosHttp.success) {
          localStorage.setItem(encode('logToken'), encode(data.mensaje));
          this.bsModalChangePinPass?.hide();
          this.injector.get(Router).navigate(['ew', 'dashboard']);
        } else{
          Swal.fire({
            title: data.titulo,
            icon: data.icono,
            html: data.mensaje
          });
        }
      },
      error: (error) => {
        this.bsModalChangePinPass?.hide();
        Notify.failure(error.message);
        this.injector.get(Router).navigate(['public']);
      }
    })
  }
}

