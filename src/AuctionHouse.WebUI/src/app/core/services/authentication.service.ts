import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'token';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private _isLoggedIn = new BehaviorSubject<boolean>(false);
  public isLoggedIn = this._isLoggedIn.asObservable();

  constructor(private router: Router, private http: HttpClient) {
      const token = sessionStorage.getItem(TOKEN_KEY);
      this._isLoggedIn.next(!!token && this.isTokenExpired(token));
  }

  public register(email: string, username: string, password: string) {
    return this.http.post<any>(`${environment.apiUrl}/user`, { email, username, password });
  }

  public login(email: string, password: string) {
    return this.http.post<any>(`${environment.apiUrl}/user/login`, { email, password })
      .pipe(map(response => {
        sessionStorage.setItem(TOKEN_KEY, JSON.stringify(response.token));
        this._isLoggedIn.next(true);
    }));
  }

  public refreshToken() {
    return this.http.post<any>(`${environment.apiUrl}/user/refresh`, { token: sessionStorage.getItem(TOKEN_KEY) })
      .pipe(map(response => {
        sessionStorage.setItem(TOKEN_KEY, JSON.stringify(response.token));
        this._isLoggedIn.next(true);
    }));
  }

  public logout() {
    sessionStorage.removeItem(TOKEN_KEY);
    this._isLoggedIn.next(false);
    this.router.navigate(['/']);
  }

  public getToken() {
    return sessionStorage.getItem(TOKEN_KEY);
  }

  private isTokenExpired(token: string) {
    const expiry = (JSON.parse(atob(token.split('.')[1]))).exp;
    return expiry * 1000 > Date.now();
  }
}