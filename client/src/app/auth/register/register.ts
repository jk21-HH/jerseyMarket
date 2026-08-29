import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../auth';
import { getErrorMessage } from '../../shared/http-error';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);

  // mirrors UserRegisterRequestDto validation on the backend
  protected readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(20)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  protected submit(): void {
    if (this.form.invalid) {
      // surfaces validation messages for untouched fields on submit attempt
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/login');
      },
      error: (err) => {
        this.error.set(getErrorMessage(err));
        this.submitting.set(false);
      },
    });
  }
}
