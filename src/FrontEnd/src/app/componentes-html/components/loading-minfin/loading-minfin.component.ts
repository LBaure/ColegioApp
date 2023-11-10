import { Component, Input, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'loading-minfin',
  templateUrl: './loading-minfin.component.html',
  styleUrls: ['./loading-minfin.component.css']
})
export class LoadingMinfinComponent implements OnInit,  OnDestroy {

  @Input() loading : boolean = false;
  @Input() src : string = '../../../../assets/logo/sirbie.png'
  @Input() label : string = 'Cargando, por favor espere...'

  constructor() { }

  ngOnDestroy(): void {
    // se reinician las variables
    this.loading = false;
    this.src = '../../../../assets/logo/sirbie.png'
    this.label = 'Cargando, por favor espere...'
  }

  ngOnInit(): void {
  }
}
