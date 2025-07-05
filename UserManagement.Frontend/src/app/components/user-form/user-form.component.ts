import { Component, Inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
export class UserFormComponent implements OnInit {
  userForm!: FormGroup;
  loading = false;
  isEdit: boolean;

  constructor(
    private fb: FormBuilder,
    private userApi: UserApiService,
    private dialogRef: MatDialogRef<UserFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UserFormData,
    private validationService: ValidationMessageService
  ) {
    this.isEdit = data.isEdit;
  }

  ngOnInit(): void {
    this.initForm();
    if (this.isEdit && this.data.user) {
      this.userForm.patchValue(this.data.user);
    }
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

      if (this.isEdit && this.data.user) {
        this.userApi.updateUser(this.data.user.id, userData).subscribe({
          next: () => {
            this.dialogRef.close(true);
            this.loading = false;
          },
          error: () => {
            this.loading = false;
          },
        });
      } else {
        this.userApi.createUser(userData).subscribe({
          next: () => {
            this.dialogRef.close(true);
            this.loading = false;
          },
          error: () => {
            this.loading = false;
          },
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getErrorMessage(control: AbstractControl, fieldName: string): string {
    return this.validationService.getErrorMessage(control, fieldName);
  }
}
