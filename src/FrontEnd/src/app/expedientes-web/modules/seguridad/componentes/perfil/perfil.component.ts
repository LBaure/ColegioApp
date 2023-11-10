import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IRespuestaHttp } from 'src/app/expedientes-web/interfaces/compartido/resultado-http';
import { IMiUsuario, IUsuario } from 'src/app/expedientes-web/interfaces/seguridad/usuario';
import { AuthService } from 'src/app/utils/auth.service';
import { SeguridadService } from '../../seguridad.service';
import Swal from 'sweetalert2';
import { EstadosHttp } from 'src/app/expedientes-web/constants/estado-http';
import { Subscription } from 'rxjs';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';
import { ResizeService } from 'src/app/utils/resize.service';
import { Notify } from 'notiflix';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  formProfile : FormGroup = {} as FormGroup;
  @Input() usuario : IUsuario = {} as IUsuario;
  @Output() newItemEvent = new EventEmitter<number>();
  servicesPantalla$ : Subscription = new Subscription;
  mq : SCREEN_SIZE = SCREEN_SIZE.LG;

  userLogin : any;
  constructor(
    private fb: FormBuilder,
    private auth : AuthService,
    private http : SeguridadService,
    private resizeService : ResizeService,
  ) {
    this.formProfile = this.getFormProfile();
    this.userLogin = this.auth.obtenerUsuario();

  }
  ngOnChanges(changes: SimpleChanges): void {
    this.formProfile.patchValue(this.usuario)
  }

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})

  }

  private getFormProfile() {
    return this.fb.group({
      nitUsuario: [{value: '', disabled:true }, Validators.required],
      nombreCompleto: [null, Validators.required],
      emailInstitucional: [{value: '', disabled:true }, Validators.required],
      emailPersonal: [null, [Validators.required, Validators.pattern('^[a-z]+[a-z0-9._-]+@[a-z]+\.[a-z.]{2,8}$')]],
      institucion: [{value: '', disabled:true }],
      unidadEjecutora: [{value: '', disabled:true }],
      cargo: [{value: '', disabled:true }],
      codigoEmpleado: [{value: '', disabled:true }],
      telefono: [null, Validators.required]
    })
  }

  btnSaveProfile() {
    if (!this.formProfile.valid) {
      Object.values(this.formProfile.controls).forEach(el => {
        el.markAsTouched();
      })
      return;
    }

    const reqProfile : IMiUsuario = {
      nitUsuario: this.usuario.nitUsuario ? this.usuario.nitUsuario : this.nitUsuario?.value,
      nombreCompleto: this.nombreCompleto?.value,
      emailPersonal: this.emailPersonal?.value,
      telefono: this.telefono?.value
    }

    console.log("reqProfile", reqProfile);


    this.http.actualizarMiPerfil(reqProfile).subscribe({
      next: (data: IRespuestaHttp) => { this.administrarRespuesta(data);},
      error: () => { },
      complete: () => {  }
    })
  }


  private administrarRespuesta(resultado:IRespuestaHttp) : void {
    if(resultado.estado === EstadosHttp.success) {
      // this.bsModalRef.hide();
      this.newItemEvent.emit(1);
    }

    if (this.mq === SCREEN_SIZE.XS || this.mq === SCREEN_SIZE.SM) {
      Notify[resultado.estado as keyof typeof Notify](resultado.mensaje, {
        position: 'center-center',
        fontSize: 'var(--bs-body-font-size)',
        clickToClose: true,
      })
    }
    else {
      Swal.fire({
        icon: resultado.icono,
        title: resultado.titulo,
        text: resultado.mensaje,
        showCloseButton: true
      });
    }
  }



  public get nitUsuario() { return this.formProfile.get('nitUsuario');}
  public get nombreCompleto() { return this.formProfile.get('nombreCompleto');}
  public get emailInstitucional() { return this.formProfile.get('emailInstitucional');}
  public get emailPersonal() { return this.formProfile.get('emailPersonal');}
  public get telefono() { return this.formProfile.get('telefono');}

}
