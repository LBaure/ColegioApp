import { Component, Input, OnInit } from '@angular/core';
import { PaginationInstance } from 'ngx-pagination';
import { NgModule } from '@angular/core';

import * as data from '../../../files/catalogos.json';
@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})


export class PaginationComponent implements OnInit {
  public page : number = 1;
  public page2: number = 1;
  public page3 : number = 1;
  public directionLinks: boolean = true;
  public itemsPage: number = 5;
  public currentPageModel: number = 1;
  public maxSizeModel: number = 5;
  public filterModel : string = '';
  public previousLabelModel : string = '';
  public nextLabelModel : string = '';



  itemsCatalogos: ICatalogos[] =  (data as any).default;

  constructor() { }

  ngOnInit(): void {
  }
  validar() {
    console.log("entro");

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
