import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { Auth } from './auth/auth';
import { authInterceptor } from './auth/auth-interceptor';
import { refreshInterceptor } from './auth/refresh-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, refreshInterceptor])),
    // silently restore a session from the stored refresh token before the app renders,
    // so a page reload doesn't bounce an authenticated user out to /login
    provideAppInitializer(() => firstValueFrom(inject(Auth).restoreSession())),
  ],
};
