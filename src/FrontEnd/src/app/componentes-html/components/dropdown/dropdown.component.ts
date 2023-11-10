import { Component, OnInit } from '@angular/core';
import * as data from '../../../files/catalogos.json';
import { ICatalogos } from '../../interfaces/models';

@Component({
  selector: 'app-dropdown',
  templateUrl: './dropdown.component.html',
  styleUrls: ['./dropdown.component.css']
})
export class DropdownComponent implements OnInit {
  showDropdown : boolean = true;
  showCodeDropdown : boolean = true;
  showCodeDropdownBtn : boolean = true;
  showCodeDropdownList : boolean = true;
  itemsCatalogos: ICatalogos[] =  (data as any).default;

  constructor() { this.itemsCatalogos = this.itemsCatalogos.splice(0,5)}

  ngOnInit(): void {
  }

  eventOpen() {
    let btn = document.getElementById("dropdownMenuButton3")
    console.log("btn", btn);


  }

}
