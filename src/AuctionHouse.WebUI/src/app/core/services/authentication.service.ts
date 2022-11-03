import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'token';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {

    constructor(private router: Router, private http: HttpClient) {}

    public register(email: string, username: string, password: string) {
      return this.http.post<any>(`${environment.apiUrl}/user`, { email, username, password });
    }

    public login(email: string, password: string) {
      return this.http.post<any>(`${environment.apiUrl}/user/login`, { email, password })
        .pipe(map(response => {
          sessionStorage.setItem(TOKEN_KEY, JSON.stringify(response.token));
      }));
    }

    public logout() {
      sessionStorage.removeItem(TOKEN_KEY);
      this.router.navigate(['/']);
    }

    public isLoggedIn(): boolean {
      const user = sessionStorage.getItem(TOKEN_KEY);
      return (user !== null) ? true : false;
    }

    public getToken() {
      return sessionStorage.getItem(TOKEN_KEY);
    }

    public refreshToken() {
      return this.http.post<any>(`${environment.apiUrl}/user/refresh`, { token: sessionStorage.getItem(TOKEN_KEY) })
        .pipe(map(response => {
          sessionStorage.setItem(TOKEN_KEY, JSON.stringify(response.token));
      }));
    }
}