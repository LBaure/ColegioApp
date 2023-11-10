import { Component, OnInit } from '@angular/core';
import { localStorageCore } from 'src/app/utils/functions/localStorageCore';
import { StartupConfigurationService } from 'src/app/utils/startup-configuration.service';

@Component({
  selector: 'app-public-app',
  templateUrl: './public-app.component.html',
  styleUrls: ['./public-app.component.css']
})
export class PublicAppComponent implements OnInit {
  sesionValid: string = "";
  public strIconoLogin : string = "Login"
  public urlNagivate : string = "login"
  constructor(
    private config : StartupConfigurationService,
    private localStorage : localStorageCore
  ) {
    this.sesionValid = this.localStorage.getItem("logToken");
  }

  ngOnInit(): void {
    if (this.config.usuario.isAuthenticated && this.sesionValid) {
      this.strIconoLogin = "Dashboard"
      this.urlNagivate = "/admin/dashboard"
    } else {
      this.strIconoLogin = "Login"
      this.urlNagivate = "/login"
    }
  }
}
