import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class ValidationMessageService {
  getErrorMessage(control: AbstractControl, fieldName?: string): string {
    if (!control.errors) return '';

    const errors = control.errors;
    const field = fieldName || 'This field';

    if (errors['required']) {
      return `${field} is required`;
    }

    if (errors['email']) {
      return 'Please enter a valid email address';
    }

    if (errors['minlength']) {
      const error = errors['minlength'];
      return `${field} must be at least ${error.requiredLength} characters long`;
    }

    if (errors['maxlength']) {
      const error = errors['maxlength'];
      return `${field} must not exceed ${error.requiredLength} characters`;
    }

    if (errors['pattern']) {
      const error = errors['pattern'];
      return `${field} ${error.requiredPattern}`;
    }

    return `${field} is invalid`;
  }
}
