import { Component, EventEmitter, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/utils/auth.service';
import { ResizeService } from 'src/app/utils/resize.service';
import { SCREEN_SIZE } from 'src/app/utils/screen-size.enum';

@Component({
  selector: 'app-side-bar',
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.css']
})
export class SideBarComponent {
  private servicesPantalla$ : Subscription = new Subscription;
  public menu : any[] = [];
  public opcion = {}
  mq? : SCREEN_SIZE;
  @Output() closeSide = new EventEmitter();

  constructor(
    private auth: AuthService,
    private resizeService : ResizeService
  ) {}

  ngOnInit(): void {
    this.resizeService.init();
    this.servicesPantalla$ = this.resizeService.observar().subscribe(x => { this.mq = x;})


    let miMenu = this.auth.obtenerMenuUsuario();
    setTimeout(() => {
      if (miMenu.length) {
        let collapse = document.getElementsByClassName('collapse border-list')
        collapse[0].classList.add('show')
      }
    }, 0);
    this.menu.push(...miMenu)
  }

  toggle() {
    let titulo = document.getElementById("info")
    titulo?.classList.remove("header-info-direccion")
    this.closeSide.emit(0);
  }
  validar(path : string) {
    if (this.mq === SCREEN_SIZE.XS || this.mq === SCREEN_SIZE.SM) {
      this.closeSide.emit(0);
    }


  }

}
