import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NavigationEnd, RouteConfigLoadEnd, Router } from '@angular/router';
import { localStorageCore } from './functions/localStorageCore';
import { catchError, of, tap } from 'rxjs';
import { UsuarioSSOModelo } from '../interfaces/usuario';
import { userCredentialsModel } from '../expedientes-web/interfaces/usuario';
import { ICredencialesUsuario } from '../expedientes-web/interfaces/Login';
import { EstadosHttp } from '../expedientes-web/constants/estado-http';

@Injectable({
  providedIn: 'root'
})
export class StartupConfigurationService {
  public usuario: UsuarioSSOModelo = {} as UsuarioSSOModelo;

  constructor(
    private http: HttpClient,
    private router: Router,
    private localStorage: localStorageCore
  ) { }

  public load() {
    return this.http.get<UsuarioSSOModelo>('/api/sso/usuario')
    .pipe(
       tap(usuario => {
        this.usuario = (usuario) ? usuario : {} as UsuarioSSOModelo;
       }),
       catchError( error => {
        this.router.navigate(['off-line']);
        return of(error)
       })
    );
  }

  private validarNavegacion(estado : EstadosHttp) {
    debugger
    console.log("validarNavegacion");

    // si se inicia la navegacion por primera vez.
    if (!localStorage.length && estado === EstadosHttp.error) {
      this.router.navigate(["/public"])
      return;
    }

    // si se a perdido la conexion y esta navegando en la app privada
    if (estado === EstadosHttp.error) {
      this.router.navigate(['off-line']);
      return;
    }

    // Si se recupera la sesion.
    if (estado === EstadosHttp.success) {
      let ultimaRuta:string = this.localStorage.getItem("lastRoute");
      if (ultimaRuta) {
        this.router.navigate([ultimaRuta])
      } else {
        this.router.navigate([""])
      }
    }
  }


  public logout () {
    const solicitud : userCredentialsModel = {
      nitUsuario: this.usuario.nit,
      valor: this.localStorage.getItem("logToken")
    };

    localStorage.clear();

    this.http.post('/api/seguridad/usuarios/logout', solicitud).subscribe({
      next: (data) => {
        this.http.get<UsuarioSSOModelo>('api/sso/usuario').toPromise();
      },
      error: (err) => {},
      complete: () => {
        localStorage.clear();
        this.usuario = {} as UsuarioSSOModelo;
        this.router.navigate(['']);
      }
    })
  }


  obtenerSesionActual() : Promise<any> {
    const reqLogout : ICredencialesUsuario = {
      nitUsuario: this.usuario.nit,
      valor: this.localStorage.getItem("logToken")
    };

    let promise = new Promise((resolve, reject) => {
      this.http.post('/api/seguridad/usuarios/config-usuario', reqLogout)
        .toPromise()
        .then(
          res => { resolve(res) },
          error =>{ reject(error.error) }
        )
    })

    return promise;
  }
}
