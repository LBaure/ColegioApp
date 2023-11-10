import { Component, OnInit } from '@angular/core';
import * as data from '../../../files/catalogos.json';
import { IBreadcrumb } from '../../interfaces/models';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  items = new Array(5);
  itemsCatalogos: ICatalogos[] =  (data as any).default;
  itemsRoutes: IBreadcrumb[] = [];

  constructor() {
    this.itemsCatalogos = this.itemsCatalogos.splice(0,5)
    this.itemsRoutes = [
      { title: "Inicio", path: "../"},
      { title: "Listas", path: "", disabled: true }
    ]
   }

  ngOnInit(): void {
  }

}

interface ICatalogos {
  idCatalogo : number;
  nombre : string;
  descripcion? : string;
  tipoCatalogo? : number;
  activo? : number;
  tipoCatalogoTexto? : string;
}
