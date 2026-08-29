import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { Products } from '../products';
import { getErrorMessage } from '../../shared/http-error';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.css',
})
export class ProductForm {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(Products);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  // no id in the route means we're creating a new product rather than editing one
  private readonly productId = Number(this.route.snapshot.paramMap.get('id')) || null;
  protected readonly isEditMode = this.productId !== null;

  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);

  // mirrors CreateProductRequestDto/UpdateProductRequestDto validation on the backend
  protected readonly form = this.fb.nonNullable.group({
    productName: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    unitsInStock: [0, [Validators.required, Validators.min(0)]],
    categoryId: [0, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    if (this.productId !== null) {
      this.productsService.getById(this.productId).subscribe((product) => {
        this.form.patchValue(product);
      });
    }
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const request = this.form.getRawValue();
    const save$ =
      this.productId !== null
        ? this.productsService.update(this.productId, request)
        : this.productsService.create(request);

    save$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        this.error.set(getErrorMessage(err));
        this.submitting.set(false);
      },
    });
  }
}
