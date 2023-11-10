import { Component, OnInit } from '@angular/core';
import { IFasesExpediente } from 'src/app/expedientes-web/interfaces/dashboard/tipos-expedientes';

@Component({
  selector: 'app-modal-filtrar-fases',
  templateUrl: './modal-filtrar-fases.component.html',
  styleUrls: ['./modal-filtrar-fases.component.css']
})
export class ModalFiltrarFasesComponent implements OnInit {

  listaFases : IFasesExpediente[] = []
  listaFasesMostrar: IFasesExpediente[] = []
  ngOnInit(): void {
    //console.log(this.listaFases);

  }

  agregarFase(item : IFasesExpediente, index: any) {

    this.listaFasesMostrar.push(item)

    this.listaFases.splice(index, 1);
  }

  quitarFase(item : IFasesExpediente, index: any) {

    this.listaFases.push(item)

    this.listaFasesMostrar.splice(index, 1);
  }
}
