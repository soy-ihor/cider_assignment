import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class UserValidators {
  static nameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const name = control.value;

      if (!name) {
        return { required: true };
      }

      if (name.trim().length < 2) {
        return {
          minlength: { requiredLength: 2, actualLength: name.trim().length },
        };
      }

      if (name.trim().length > 100) {
        return {
          maxlength: { requiredLength: 100, actualLength: name.trim().length },
        };
      }

      return null;
    };
  }

  static emailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const email = control.value;

      if (!email) {
        return { required: true };
      }

      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      if (!emailPattern.test(email)) {
        return { email: true };
      }

      if (email.length > 100) {
        return {
          maxlength: { requiredLength: 100, actualLength: email.length },
        };
      }

      return null;
    };
  }

  static usernameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const username = control.value;

      if (!username) {
        return { required: true };
      }

      if (username.trim().length < 3) {
        return {
          minlength: {
            requiredLength: 3,
            actualLength: username.trim().length,
          },
        };
      }

      if (username.trim().length > 50) {
        return {
          maxlength: {
            requiredLength: 50,
            actualLength: username.trim().length,
          },
        };
      }

      // Username should contain only letters, numbers, and underscores
      const usernamePattern = /^[a-zA-Z0-9_]+$/;
      if (!usernamePattern.test(username)) {
        return {
          pattern: {
            requiredPattern: 'letters, numbers, and underscores only',
          },
        };
      }

      return null;
    };
  }
}
