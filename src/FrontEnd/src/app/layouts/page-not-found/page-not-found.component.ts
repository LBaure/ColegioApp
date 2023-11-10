import { Component, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { localStorageCore } from 'src/app/utils/functions/localStorageCore';

@Component({
  selector: 'app-page-not-found',
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.css']
})
export class PageNotFoundComponent implements OnInit {

  constructor(
    private _location: Location,
    private route: ActivatedRoute,
    private router: Router,
    private localStorage: localStorageCore
  ) { }

  ngOnInit(): void {
    let lastRoute = this.localStorage.getItem("lastRoute");
    console.log("lastRoute", lastRoute);

  }

  backClicked() {
    debugger
    let lastRoute = this.localStorage.getItem("lastRoute");

    const path = lastRoute ? lastRoute : '/sw/dashboard'


    this.router.navigate([path]);
  }

}
