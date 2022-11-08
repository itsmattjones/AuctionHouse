import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private isLoggedIn: boolean;

    constructor(private authenticationService: AuthenticationService) {
        this.authenticationService.isLoggedIn.subscribe(val => this.isLoggedIn = val);
     }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const isApiUrl = request.url.startsWith(environment.apiUrl);

        if (isApiUrl && this.isLoggedIn) {
            request = request.clone({
                withCredentials: true,
                setHeaders: { 
                    Authorization: `Bearer ${this.authenticationService.getToken()}`
                }
            });
        }

        return next.handle(request);
    }
}