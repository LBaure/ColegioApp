import { Component, OnInit } from '@angular/core';
import { SeguridadService } from '../seguridad.service';
import { AuthService } from 'src/app/utils/auth.service';
import { IUsuario } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import Swal from 'sweetalert2';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { Notify } from 'notiflix';

@Component({
  selector: 'app-mi-perfil',
  templateUrl: './mi-perfil.component.html',
  styleUrls: ['./mi-perfil.component.css']
})
export class MiPerfilComponent implements OnInit {
  private userLogin : any;
  public usuario : IUsuario = {} as IUsuario;
  nombreUsuario : string = "";
  nombrePuesto: string = "";
  lockTab: boolean = false;
  constructor(
    private http: SeguridadService,
    private auth : AuthService
  ) {
    this.userLogin = this.auth.obtenerUsuario()
  }

  ngOnInit(): void {
    this.getUserInformation();
  }

  getUserInformation () {
    this.http.obtenerUsuarios(this.userLogin.nit).subscribe({
      next: (data) => {
        if (data.length) {
          this.usuario = {...data[0]}
          this.nombreUsuario = this.usuario.nombreCompleto;
          this.nombrePuesto = this.usuario.cargo;
          if (!this.usuario.existePolitica) {
            this.lockTabChangePinPass();
          }
        }
      },
      error : (error) => { Notify.failure(error.message); }
    })
  }

  actualizarInformacion(response: number) {
    if (response === 1) {
      this.getUserInformation();
    }
  }

  lockTabChangePinPass() : void {
    var tab = document.getElementById('pills-pin-pass-tab');
    let contextThis = this;
    tab?.addEventListener("click", function (evt) {
      Swal.fire({
        icon: EstadosHttp.info,
        text: "Debe completar la pestaña de politica de privacidad primero.",
        showCloseButton: true
      })
      contextThis.lockTab = true;
     })
  }

}
