import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    private isLoggedIn: boolean;

    constructor(private authenticationService: AuthenticationService) { 
        this.authenticationService.isLoggedIn.subscribe(val => this.isLoggedIn = val);
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status == 401 && this.isLoggedIn) {
                this.authenticationService.refreshToken().pipe(
                    switchMap(() => {
                        return next.handle(request);
                    }),
                    catchError((error) => {
                        this.authenticationService.logout();
                        return throwError(() => error);
                    }));
            }

            const error = err.error.message || err.statusText;
            return throwError(() => error);
        }))
    }
}