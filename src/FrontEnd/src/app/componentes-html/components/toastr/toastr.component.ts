import { Component, OnInit } from '@angular/core';
import { Loading, Notify } from 'notiflix';


@Component({
  selector: 'app-toastr',
  templateUrl: './toastr.component.html',
  styleUrls: ['./toastr.component.css']
})
export class ToastrComponent implements OnInit {
  showNotifications: boolean = true;
  showNotifyCustom: boolean = true;
  showLoading: boolean = true;
  showLoadingCustom: boolean = true;

  loading : boolean = false;

  documentacion : any[] = [
    {
      opcion: "loading",
      tipo: "boolean",
      valorDefecto: false,
      descripcion: "Habilita o deshabilita el loading."
    },
    {
      opcion: "label",
      tipo: "string",
      valorDefecto: "Cargando, por favor espere...",
      descripcion: "Cambia el mensaje situado arriba del icono cargando."
    },
    {
      opcion: "loading",
      tipo: "string",
      valorDefecto: "	fondo-loading.png",
      descripcion: "Cambia la imagen situada arriba del icono cargando."
    }

  ]
  constructor() { }

  ngOnInit(): void {

  }

  notify(typeNotify : string) {
    Notify[typeNotify as keyof typeof Notify](`Noficacion estilo ${typeNotify}`);
  }

  /**
   * notifyCustom
   */
  public notifyCustom() {
    Notify.success('Notificacion personalizada, posición, borde radius, boton cerrar', {
      position: 'center-center',
      borderRadius: '4px',
      closeButton: true,
      });

  }

  /**
   * loadingStart
   */
  public loadingStart() {
    Loading.standard()
    setTimeout(() => {
      Loading.remove()
    }, 3000);
  }


  /**
   * loadingDots
   *
   */
  public loadingDots() {
    Loading.dots('Cargando, por favor espere...');
    setTimeout(() => {
      Loading.remove()
    }, 3000);
    // }, 1 * 60 * 1000);
  }


  /**
   * openCustomLoad()
   */
  public openCustomLoad() {
    this.loading = true;
    setTimeout(() => {
      this.loading = false;
    }, 3000);

  }



}
