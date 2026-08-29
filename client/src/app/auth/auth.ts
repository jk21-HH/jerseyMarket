import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';

import {
  AccessTokenRefreshTokenResponse,
  UserLoginRequest,
  UserRegisterRequest,
  UserResponse,
} from './auth.model';
import { getUserIdFromToken } from './jwt';

const REFRESH_TOKEN_KEY = 'refreshToken';
const USER_ID_KEY = 'userId';

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
    return this.http
      .post<AccessTokenRefreshTokenResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap((res) => this.applySession(res)));
  }

  // notifies the backend to revoke the refresh token, then always clears local session state
  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, {}).pipe(
      catchError(() => of(undefined)),
      tap(() => this.clearSession()),
      map(() => undefined),
    );
  }

  private clearSession(): void {
    this.accessTokenSignal.set(null);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_ID_KEY);
  }

  // called once on app startup to silently restore a session after a page reload
  restoreSession(): Observable<boolean> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    const userId = localStorage.getItem(USER_ID_KEY);

    if (!refreshToken || !userId) {
      return of(false);
    }

    return this.http
      .post<AccessTokenRefreshTokenResponse>(`${this.baseUrl}/regenerate-tokens`, {
        userId: Number(userId),
        refreshToken,
      })
      .pipe(
        tap((res) => this.applySession(res)),
        map(() => true),
        catchError(() => {
          this.clearSession();
          return of(false);
        }),
      );
  }

  get accessToken(): string | null {
    return this.accessTokenSignal();
  }

  private applySession(res: AccessTokenRefreshTokenResponse): void {
    this.accessTokenSignal.set(res.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, res.refreshToken);

    const userId = getUserIdFromToken(res.accessToken);
    if (userId !== null) {
      localStorage.setItem(USER_ID_KEY, String(userId));
    }
  }
}
