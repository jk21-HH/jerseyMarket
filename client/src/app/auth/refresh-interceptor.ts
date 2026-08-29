import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

import { Auth } from './auth';

// on a 401 (expired access token), silently refresh once and retry the request;
// skip Auth endpoints themselves to avoid retrying a failed login/refresh in a loop
export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(Auth);

  return next(req).pipe(
    catchError((err) => {
      if (!(err instanceof HttpErrorResponse) || err.status !== 401 || req.url.includes('/api/Auth/')) {
        return throwError(() => err);
      }

      return auth.refreshAccessToken().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            return throwError(() => err);
          }

          const accessToken = auth.accessToken;
          return next(
            req.clone({
              setHeaders: { Authorization: `Bearer ${accessToken}` },
            }),
          );
        }),
      );
    }),
  );
};
