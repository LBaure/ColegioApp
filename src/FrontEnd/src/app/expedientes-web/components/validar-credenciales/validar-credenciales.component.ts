import { Component, ElementRef, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { encode } from 'src/app/utils/functions/encode';
import { ResizeService } from 'src/app/utils/resize.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { IPoliticaPrivacidad } from '../../interfaces/seguridad/politica-privacidad';

@Component({
  selector: 'app-validar-credenciales',
  templateUrl: './validar-credenciales.component.html',
  styleUrls: ['./validar-credenciales.component.css']
})
export class ValidarCredencialesComponent implements OnInit, OnDestroy {
  @ViewChild('credencial') inputCredencial: ElementRef = {} as ElementRef;
  servicesPantalla$ : Subscription = new Subscription;
  descripcion: string = "";
  input: IPoliticaPrivacidad = {} as IPoliticaPrivacidad;

  formValidarCredenciales : FormGroup = {} as FormGroup;
  public EventoGuardar = new EventEmitter<string>();

  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private resizeService : ResizeService
  ) { this.formValidarCredenciales = this.obtenerControlesFormulario(); }

  ngOnInit(): void {
    this.descripcion = `La verificación en dos pasos esta activa, ingresa tu ${ this.input.etiqueta } de seguridad, para confirmar tu identidad.`
    setTimeout(()=>{
      this.inputCredencial.nativeElement.focus();
    },0);


    // control del tamaño de la pantalla, respecto a la modal
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => {
      if (x === SCREEN_SIZE.XS || x === SCREEN_SIZE.SM) {
        this.bsModalRef.setClass('modal-fullscreen modal-dialog-centered')
      } else {
        this.bsModalRef.setClass('modal-md modal-dialog-centered')
      }
    })
  }

  obtenerControlesFormulario() : FormGroup {
    return this.fb.group({
      valor: [null, Validators.required]
    })
  }




  getMax(evt:any, val:number) {
    const txt =  evt.target.value
    if (txt.length == val) {
      return false;
    }
    return true
  }

  enviarValor() {
    if (!this.formValidarCredenciales.valid) {
      Object.values(this.formValidarCredenciales.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }
    this.EventoGuardar.emit(encode(this.newValue?.value))
  }

  cambiarIconoContrasenia(inputId:any, inputIcon: any) {
    var input = document.getElementById(inputId as keyof HTMLElement)
    var span = document.getElementById(inputIcon)

    if (input && span) {
      if (input["type" as keyof HTMLElement] == "password") {
        input.setAttribute('type', 'text');
        span.innerHTML= "visibility"
      } else {
        input.setAttribute('type', 'password');
        span.innerHTML= "visibility_off"
      }
    }

  }




  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
  }


  public get newValue() { return this.formValidarCredenciales.get('valor'); }

}
