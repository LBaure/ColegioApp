import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './layouts/components/header/header.component';
import { FooterComponent } from './layouts/components/footer/footer.component';
import { SideBarComponent } from './layouts/components/side-bar/side-bar.component';
import { AutoFocusDirective } from './utils/auto-focus.directive';
import { AuthService } from './utils/auth.service';
import { ActivatedRouteSnapshot } from '@angular/router';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { StartupConfigurationService } from './utils/startup-configuration.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from  '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NotFoundComponent } from './layouts/not-found/not-found.component';
import { PageBadGatewayComponent } from './layouts/page-bad-gateway/page-bad-gateway.component';
import { RedirectionComponent } from './redirection/redirection.component';
import { RedirectToComponent } from './redirect-to/redirect-to.component';
import { ComponentesHtmlModule } from './componentes-html/componentes-html.module';
import { LoadingComponent } from './layouts/components/loading/loading.component';
import { LoadingInterceptor } from './layouts/components/loading/loading.interceptor';
import { AuthInterceptor } from './expedientes-web/utils/auth.interceptor';
import { PublicAppComponent } from './layouts/public-app/public-app.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { IConfig, provideEnvironmentNgxMask } from 'ngx-mask';
import { ExpedientesWebLayoutComponent } from './layouts/expedientes-web-layout/expedientes-web-layout.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { LoginComponent } from './layouts/login/login.component'

const maskConfig: Partial<IConfig> = {
  validation: false,
};
function initializeAppFactory(config: StartupConfigurationService): () => Observable<any> {
  return () => config.load();
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    SideBarComponent,
    AutoFocusDirective,
    NotFoundComponent,
    PageBadGatewayComponent,
    RedirectionComponent,
    RedirectToComponent,
    LoadingComponent,
    PublicAppComponent,
    ExpedientesWebLayoutComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ComponentesHtmlModule,
    BsDropdownModule.forRoot(),
    BrowserAnimationsModule,
    TabsModule.forRoot(),
    BsDatepickerModule.forRoot(),
    TooltipModule.forRoot()
  ],
  providers: [
    provideEnvironmentNgxMask(maskConfig),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeAppFactory,
      deps: [StartupConfigurationService],
      multi: true
    },
    {
      provide: 'externalUrlRedirectResolver',
      useValue: (route: ActivatedRouteSnapshot) => {
        window.location.href = (route.data as any).externalUrl;
      }
    },
    {
      provide: LocationStrategy, useClass: HashLocationStrategy
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
],
  bootstrap: [AppComponent]
})
export class AppModule { }
