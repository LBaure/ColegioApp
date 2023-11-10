import { Component, Input, OnInit } from '@angular/core';
import { IBreadcrumb } from '../../interfaces/models';

@Component({
  selector: 'minfin-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  @Input() title : string = "Diseño breadcrumb"
  @Input() separator : string = "/";
  @Input() routes : IBreadcrumb[] = [
    { title: "Dashboard", path: "/dashboard", disabled: false },
    { title: "Inicio", path: "../", disabled: true},
    { title: "Listas", path: "/public/lists", disabled: true}
  ]
  @Input() classPath : string = "";

  constructor() { }

  ngOnInit(): void {
  }

  getDividerBreadcrumbs(index : any)  {
    let fin = this.routes.length;
    if (index == (fin - 1)) {
      return ""
    } else {
      return this.separator
    }
  }
}
const checkpoint = 20;
let opacity = 0;
window.addEventListener("scroll", () => {
  const currentScroll = window.pageYOffset;
  if (currentScroll <= checkpoint) {
    opacity = 1 - currentScroll / checkpoint;
  } else {
    opacity = 0;
  }
  let nav : any | undefined;
  nav = document.getElementById("nav-bread")
  if (nav) {
    nav.style.opacity = opacity;
  }
});
