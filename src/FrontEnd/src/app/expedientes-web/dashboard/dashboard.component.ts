import { Component, OnInit } from '@angular/core';
@Component({
  selector: 'app-dashboard-welcome',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit{
  // escribir en pantalla
  descripcion: string = "Expedientes Web 2.0, actualmente ofrece un dashboard por tipo de expedientes, usuarios asignados, fases en las que se encuentran los expedientes, entre otros."

  ngOnInit(): void {
    // let speed:number=50;
    // let p = document.querySelector('p');
    // let delay = 1 * speed + speed;

    // setTimeout(() => {
    //   p!.style.display = "block";
    //   this.typeEffect(p, speed)
    // }, delay);
  }

  typeEffect(element:any, speed:any) {
    var text = element.innerHTML;
    element.innerHTML = "";
    var i = 0;
    var timer = setInterval(function() {
      if (i < text.length) {
        element.append(text.charAt(i));
        i++;
      } else {
        clearInterval(timer);
      }
    }, speed);
  }
}
