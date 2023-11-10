import { Component, OnInit } from '@angular/core';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import * as data from '../../../files/catalogos.json';
import { IBreadcrumb } from '../../interfaces/models';

@Component({
  selector: 'app-tables',
  templateUrl: './tables.component.html',
  styleUrls: ['./tables.component.css']
})
export class TablesComponent implements OnInit {
  itemsCatalogos: ICatalogos[] =  (data as any).default;
  page: number = 1;
  mq: string = "";
  itemsRoutes: IBreadcrumb[] = [];

  constructor() {
    this.itemsRoutes = [
      { title: "Home", path: "../"},
      { title: "tablas", path: "", disabled: true }
    ]
  }

  ngOnInit() {
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
