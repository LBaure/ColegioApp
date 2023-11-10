import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthService } from 'src/app/utils/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private readonly authService: AuthService, private readonly router : Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const authToken = this.authService.obtenerToken();
    if (authToken) {
      request = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${authToken}`)
      })
    }
    return next.handle(request).pipe(
      tap({
        next: (event : any) => {
          if (event instanceof HttpResponse) {
            if(event.status == 401) {
              alert('Unauthorized access!')
            }
          }
          return event;
        },
        error: (error : HttpErrorResponse) => {
          /**
           * Manejo de Errores
           * Si el api retorna un estado 401, significa que el usuario no esta authorizado
           * para acceder a ese recurso o no esta logueado.
           */
          if(error.status === 401) {
            console.log("Interceptor: ", error.message);
            setTimeout(() => {
              // se configuro un delay ya que, hace una peticion al servidor y queda con freezada.
              this.authService.logout();
            }, 0);
          }
          /**
           * Si el api retorna un estado 404, significa que el recurso no esta disponible
           * para el usuario o que no ha sido programado.
           */
          else if(error.status === 404) {
            // this.router.navigate(['404'])
          }
        }
      })
    );
  }
}
