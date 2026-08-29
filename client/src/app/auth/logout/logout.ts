import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { Auth } from '../auth';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.html',
  styleUrl: './logout.css',
})
export class Logout {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  constructor() {
    this.authService.logout().subscribe(() => this.router.navigateByUrl('/'));
  }
}
