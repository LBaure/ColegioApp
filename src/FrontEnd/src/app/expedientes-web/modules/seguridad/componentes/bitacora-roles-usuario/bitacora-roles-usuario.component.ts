import { Component, OnInit } from '@angular/core';
import { IHeader } from 'src/app/expedientes-web/interfaces/header';
import { IBitacoraUsuarioRoles } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { SeguridadService } from '../../seguridad.service';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Notify } from 'notiflix';

@Component({
  selector: 'app-bitacora-roles-usuario',
  templateUrl: './bitacora-roles-usuario.component.html',
  styleUrls: ['./bitacora-roles-usuario.component.css']
})
export class BitacoraRolesUsuarioComponent implements OnInit {

  headers: IHeader[] = [
    { text: 'Nombre del Rol', width: '25%', align: 'center'},
    { text: 'Usuario que Registro', width: '30%', align: 'center'},
    { text: 'Fecha de Registro', width: '25%', align: 'center'},
    { text: 'Tipo de Permiso', width: '20%', align: 'center'}
  ];
  listadoBitacora: IBitacoraUsuarioRoles[] = [];
  nitUsuario: string = "";
  nombreUsuario: string = "";
  page : number = 1;
  itemsPage : number = 10;

  constructor(
    private http: SeguridadService,
    public bsModalRef: BsModalRef
  ) { }

  ngOnInit(): void {
    this.http.obtenerBitacoraRolesUsuario(this.nitUsuario).subscribe({
      next: (data: IRespuestaHttp) => {
        if (data.estado === EstadosHttp.success) {
          let result : IBitacoraUsuarioRoles[] = data.resultado;
          this.listadoBitacora = result;
        }
      },
      error: error => { Notify.failure(error.message); }
    })

  }

}
