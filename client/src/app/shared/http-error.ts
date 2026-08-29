import { HttpErrorResponse } from '@angular/common/http';

// the backend returns plain strings for BadRequest/Conflict/NotFound and
// ProblemDetails JSON ({ title, detail }) for unhandled exceptions - normalize both
export function getErrorMessage(err: unknown): string {
  if (!(err instanceof HttpErrorResponse)) {
    return 'An unexpected error occurred.';
  }

  if (typeof err.error === 'string' && err.error.trim().length > 0) {
    return err.error;
  }

  const problemDetails = err.error as { title?: string; detail?: string } | null;
  if (problemDetails?.detail) {
    return problemDetails.detail;
  }
  if (problemDetails?.title) {
    return problemDetails.title;
  }

  return err.message;
}
