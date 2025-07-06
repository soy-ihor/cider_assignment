import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { UserApiService } from '../../services/user-api.service';
import { User } from '../../models/user.model';
import { UserValidators } from '../../validators/user.validators';
import { ValidationMessageService } from '../../shared/services/validation-message.service';

export interface UserFormData {
  user?: User;
  isEdit: boolean;
}

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss'],
})
export class UserFormComponent implements OnInit, OnDestroy {
  userForm!: FormGroup;
  loading = false;
  isEdit: boolean;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private userApi: UserApiService,
    private dialogRef: MatDialogRef<UserFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UserFormData,
    private validationService: ValidationMessageService,
    private snackBar: MatSnackBar
  ) {
    this.isEdit = data.isEdit;
  }

  ngOnInit(): void {
    this.initForm();
    if (this.isEdit && this.data.user) {
      this.userForm.patchValue(this.data.user);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initForm(): void {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, UserValidators.nameValidator()]],
      email: ['', [Validators.required, UserValidators.emailValidator()]],
      username: ['', [Validators.required, UserValidators.usernameValidator()]],
    });
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      this.loading = true;
      const userData = this.userForm.value;

      const request$ =
        this.isEdit && this.data.user
          ? this.userApi.updateUser(this.data.user.id, userData)
          : this.userApi.createUser(userData);

      request$
        .pipe(
          takeUntil(this.destroy$),
          finalize(() => (this.loading = false))
        )
        .subscribe(() => {
          this.showMessage('User updated successfully', 'success');
          this.dialogRef.close(true);
        });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getErrorMessage(controlName: string, fieldName: string): string {
    const control = this.userForm.get(controlName) as AbstractControl;
    return this.validationService.getErrorMessage(control, fieldName);
  }

  private showMessage(message: string, type: 'success' | 'error'): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass:
        type === 'success' ? ['success-snackbar'] : ['error-snackbar'],
    });
  }
}
