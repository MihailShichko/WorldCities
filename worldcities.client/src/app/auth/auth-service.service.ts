import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequest } from './LoginRequest';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginResult } from './LoginResult';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    constructor(
        protected http: HttpClient) {
    }

    private tokenKey: string = "token";

    private _authStatus = new BehaviorSubject<boolean>(false);
    public authStatus = this._authStatus.asObservable();

    isAuthentificated(): boolean {
        return this.getToken() !== null;
    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    init(): void {
        if (this.isAuthentificated())
            this.setAuthStatus(true);
    }

    login(item: LoginRequest): Observable<LoginResult> {
        var url = environment.baseUrl + "api/Account/Login";
        return this.http.post<LoginResult>(url, item)
            .pipe(tap(loginResult => {
                if (loginResult.success && loginResult.token) {
                    localStorage.setItem(this.tokenKey, loginResult.token);
                    this.setAuthStatus(true);
                }
            }));
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        this.setAuthStatus(false);
    }

    private setAuthStatus(isAuthenticated: boolean): void {
        this._authStatus.next(isAuthenticated);
    }
}
