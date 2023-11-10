import { Component, Input } from '@angular/core';
import { IHistoricoExpediente } from 'src/app/expedientes-web/interfaces/dashboard/encabezado-expediente';

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.css']
})
export class TimelineComponent {
  @Input() items: IHistoricoExpediente[] = [];



  getClass(indice:number) {
    if(indice % 2){
      return 'direction-r'
    } else {
      return 'direction-l'
    }
  }

}
