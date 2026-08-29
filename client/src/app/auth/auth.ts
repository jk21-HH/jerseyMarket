import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import {
  AccessTokenRefreshTokenResponse,
  UserLoginRequest,
  UserRegisterRequest,
  UserResponse,
} from './auth.model';

const REFRESH_TOKEN_KEY = 'refreshToken';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/Auth';

  // access token kept in memory only (never persisted) so it isn't readable via storage-based XSS
  private readonly accessTokenSignal = signal<string | null>(null);
  readonly isAuthenticated = computed(() => this.accessTokenSignal() !== null);

  register(request: UserRegisterRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.baseUrl}/register`, request);
  }

  login(request: UserLoginRequest): Observable<AccessTokenRefreshTokenResponse> {
    return this.http.post<AccessTokenRefreshTokenResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((res) => {
        this.accessTokenSignal.set(res.accessToken);
        // refresh token persists across reloads so the session can be restored via regenerate-tokens
        localStorage.setItem(REFRESH_TOKEN_KEY, res.refreshToken);
      }),
    );
  }

  logout(): void {
    this.accessTokenSignal.set(null);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }

  get accessToken(): string | null {
    return this.accessTokenSignal();
  }
}
