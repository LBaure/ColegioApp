import { Component, OnInit } from '@angular/core';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  AlertBasic() {
    Swal.fire('Esto es un ejemplo de Alerta Basica')
  }
  AlertBasicClose() {
    Swal.fire({
      title: 'Esto es un ejemplo de Alerta Basica',
      showCloseButton: true
    })
  }

  AlertTitleMessage() {
    Swal.fire({
      title: 'Titulo',
      text: 'Este es el subtitulo',
      showCloseButton: true
    })
  }

  AlertTitleMessageIcon() {
    Swal.fire({
      icon: 'info',
      title: 'Alerta',
      text: 'Incluye un Icono',
      showCloseButton: true
    })
  }


  AlertSuccess() {
    Swal.fire({
      icon: 'success',
      title: 'Alerta',
      text: 'Los cambios se han guardado exitosamente.',
      showCloseButton: true
    })
  }

  alertConfirm() {
    Swal.fire({
      icon: "warning",
      title: '¿Desea eliminar el registro?',
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      /* Read more about isConfirmed, isDenied below */
      if (result.isConfirmed) {
        Swal.fire('Registro eliminado correctamente!', '', 'success')
      } else if (result.isDenied) {
        Swal.fire('Changes are not saved', '', 'info')
      }
    })
  }

  AlertButtonClose() {
      Swal.fire({
      title: '<strong>HTML <u>example</u></strong>',
      icon: 'info',
      iconColor: "var(--bs-primary)",
      html:
        'You can use <b>bold text</b>, ' +
        '<a href="//sweetalert2.github.io">links</a> ' +
        'and other HTML tags',
      showCloseButton: true,
      showCancelButton: true,
      focusConfirm: false,
      confirmButtonText:
        '<i class="bi bi-hand-thumbs-up"></i> Great!',
      confirmButtonAriaLabel: 'Thumbs up, great!',
      cancelButtonText:
        '<i class="bi bi-hand-thumbs-down"></i>',
      cancelButtonAriaLabel: 'Thumbs down',
      customClass: {
        closeButton:"alertClose"
      }
    })
  }

}
