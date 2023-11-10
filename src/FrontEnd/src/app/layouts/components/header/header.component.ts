import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioSSOModelo } from 'src/app/interfaces/usuario';
import { AuthService } from 'src/app/utils/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  iconMode: string = "bi bi-moon-stars"
  usuario : UsuarioSSOModelo = {} as UsuarioSSOModelo;

  name:string = "";
  showTitle : boolean = false;
  @Output() open = new EventEmitter();
  @Output() dark = new EventEmitter();
  @Input() iconMenu: string = 'menu';

  constructor(
    // private http: HttpClient,
    private router: Router,
    private auth: AuthService,
  ) {
    this.usuario = auth.obtenerUsuario();
    // this.user = {
    //   name: "Luis Bautista",
    //   nit: '63397323'
    // }
  }

  ngOnInit(): void {
    if(this.usuario) {
      // Se transforma el string user -> a un array
      let userArr = this.usuario.name.split(" ")
      // se extrae la primer posicion, esta sera el nombre que se
      // muestre en el menú del usuario
      this.name = userArr[0];
    }
  }

  ngAfterViewInit() {
    Promise.resolve().then(() => {
      if (localStorage.getItem('data-theme')) {
        document.documentElement.setAttribute('data-theme', 'dark-mode');
        this.iconMode = "bi bi-brightness-high";
        document.body.classList.add('dark-mode')
        this.dark.emit(1);
      }
    })
  }

  toggle() {
    if (this.iconMenu === "menu") {
      this.open.emit(1);
      let titulo = document.getElementById("info")
      titulo?.classList.add("header-info-direccion")
    } else {
      this.open.emit(0);
      let titulo = document.getElementById("info")
      titulo?.classList.remove("header-info-direccion")
    }
  }

  cambiarApariencia() {
    /**
     * Valida si el documento html tiene el atributo data-theme, este lo usaremos para el modo oscuro
     * y se cambiara el valor del icono en el navbar y enviara un evento al componente padre
     */
    if (document.documentElement.hasAttribute('data-theme')) {
      // Elimina el elemento del documento -> el modo sera normal
      document.documentElement.removeAttribute('data-theme');
      // Cambia el valor del icono este mostrara el modo obscuro
      this.iconMode = "bi bi-moon-stars";
      // Envia un evento al componente padre -> <marco-general></marco-general>
      this.dark.emit(0);
      // Elimina el localStorage almacenado
      localStorage.removeItem('data-theme');
      document.body.classList.remove('dark-mode')
    }
    else {
      // Activa el modo obscuro
      document.documentElement.setAttribute('data-theme', 'dark-mode');
      // Cambia el valor del icono, este mostrara el modo light
      this.iconMode = "bi bi-brightness-high";
      // Envia un evento al componente padre -> <marco-general></marco-general>
      this.dark.emit(1);
      // Almacena en localStorage el atributo y su valor
      localStorage.setItem('data-theme', 'dark-mode');
      document.body.classList.add('dark-mode')
    }
  }

  logout() {
    this.auth.logout();
  }
}
