import { Component } from '@angular/core';

import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  version:string = '';
  direccion: string = '';
  telefono: string = ''
  nombreSistema: string = '';
  /**
   *
   */
  constructor() {
    this.version = environment.version;
    this.direccion = environment.direccion;
    this.telefono = environment.telefono;
    this.nombreSistema = environment.nombreSistema;
  }

}
