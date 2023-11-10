import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthGuardService } from 'src/app/utils/auth-guard.service';
import { AuthService } from 'src/app/utils/auth.service';
import { localStorageCore } from 'src/app/utils/functions/localStorageCore';

@Component({
  selector: 'app-page-bad-gateway',
  templateUrl: './page-bad-gateway.component.html',
  styleUrls: ['./page-bad-gateway.component.css']
})
export class PageBadGatewayComponent implements OnInit {
  /**
   *
   */
  constructor(
    private localStorage: localStorageCore,
    private auth: AuthService
  ) { }

  ngOnInit(): void {
    let lastRoute = this.localStorage.getItem("lastRoute");
    if (this.auth.isAuthenticated()) {
      const path:string = lastRoute ? lastRoute : '';
      this.auth.navigate(path);
    } else {
      this.auth.navigate('login')
    }
  }

  recargar() {
    location.reload();
  }

}
