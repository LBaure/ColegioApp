import { Component, OnInit, TemplateRef  } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-modals',
  templateUrl: './modals.component.html',
  styleUrls: ['./modals.component.css']
})
export class ModalsComponent implements OnInit {
  modalRef?: BsModalRef;
  modalRefSize?:BsModalRef;
  modalRefCustom?:BsModalRef;
  config = {
    class: '',
    backdrop: true,
    ignoreBackdropClick: true
  };
  constructor(
    private modalService: BsModalService
  ) {}


  ngOnInit(): void {
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  openModalSize(template: TemplateRef<any>, size: string = 'modal-xxl') {
    this.modalRefSize = this.modalService.show(template, {class: size});
  }
  openModalBackdrop(template: TemplateRef<any>, clase : string = '') {
    this.config.class = clase
    this.modalRef = this.modalService.show(template, this.config);
  }

  openModalCustom(template: TemplateRef<any>, fullscreen: string = '') {
    this.modalRefCustom = this.modalService.show(template, { class: fullscreen});

  }

}
