import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-view-file',
  templateUrl: './view-file.component.html',
  styleUrls: ['./view-file.component.css']
})
export class ViewFileComponent implements OnInit {
  path: string = '';
  nameFile: string = '';
  typeFile: string = '';
  colorModal: string = 'dark';
  iconMaximize : string = 'bi bi-arrows-fullscreen'


  esImagen : boolean = false;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {

    const len = this.typeFile.indexOf('/')
    const fileType = this.typeFile.substring(0, len)
    if (fileType === 'image') {
      this.esImagen = true;
    }
  }

  maximizeModal() {
    if (this.iconMaximize === 'bi bi-arrows-fullscreen') {
      if ( this.esImagen) {
        this.bsModalRef.setClass('modal-lg modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-fullscreen')
      }
      this.iconMaximize = 'bi bi-fullscreen-exit'
    } else {
      this.bsModalRef.setClass('modal-md modal-dialog-centered')
      this.iconMaximize = 'bi bi-arrows-fullscreen'
    }
  }



}
