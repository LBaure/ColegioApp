import { Component, ElementRef, EventEmitter, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { encode } from 'src/app/utils/functions/encode';
import { ResizeService } from 'src/app/utils/resize.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { configInput } from '../../interfaces/ConfigInput';

@Component({
  selector: 'app-two-factor-auth',
  templateUrl: './two-factor-auth.component.html',
  styleUrls: ['./two-factor-auth.component.css']
})
export class TwoFactorAuthComponent implements OnInit, OnDestroy {

  @ViewChild('search') searchElement: ElementRef = {} as ElementRef;

  loading : boolean = false;
  titleModal: string = "Credenciales"
  btnCancel: boolean = true;
  configInput: configInput = {} as configInput;
  servicesPantalla$ : Subscription = new Subscription;

  public EventModel : EventEmitter<string> = new EventEmitter<string>();
  public formTwoFactor : FormGroup = {} as FormGroup;

  constructor(
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private resizeService : ResizeService
  ) { this.formTwoFactor = this.getFormPass(); }


  ngOnDestroy(): void {
    this.servicesPantalla$.unsubscribe();
  }

  ngOnInit(): void {
    setTimeout(()=>{
      this.searchElement.nativeElement.focus();
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

  get newValue() {
    return this.formTwoFactor.get('newValue');
  }

  getFormPass() : FormGroup {
    return this.fb.group({
      newValue: [null, Validators.required]
    })
  }

  btnConfirm() {
    if (!this.formTwoFactor.valid) {
      Object.values(this.formTwoFactor.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }
    this.EventModel.emit(encode(this.newValue?.value))
  }

  changeIconInput(inputId:any, inputIcon: any) {
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
  getMax(evt:any, val:number) {
    const txt =  evt.target.value
    if (txt.length == val) {
      return false;
    }
    return true
  }

}
