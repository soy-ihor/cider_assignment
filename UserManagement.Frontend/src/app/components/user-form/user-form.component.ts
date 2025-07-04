import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserApiService } from '../../services/user-api.service';
import { User, CreateUserDto, UpdateUserDto } from '../../models/user.model';

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
    @Inject(MAT_DIALOG_DATA) public data: UserFormData
  ) {
    this.isEdit = data.isEdit;
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(3)]],
    });

    if (this.isEdit && this.data.user) {
      this.userForm.patchValue({
        name: this.data.user.name,
        email: this.data.user.email,
        username: this.data.user.username,
      });
    }
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      this.loading = true;
      const formValue = this.userForm.value;

      if (this.isEdit && this.data.user) {
        const updateDto: UpdateUserDto = {
          name: formValue.name,
          email: formValue.email,
          username: formValue.username,
        };

        this.userApi.updateUser(this.data.user.id, updateDto).subscribe({
          next: (user) => {
            this.loading = false;
            this.dialogRef.close(user);
          },
          error: () => {
            this.loading = false;
          },
        });
      } else {
        const createDto: CreateUserDto = {
          name: formValue.name,
          email: formValue.email,
          username: formValue.username,
        };

        this.userApi.createUser(createDto).subscribe({
          next: (user) => {
            this.loading = false;
            this.dialogRef.close(user);
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

  getErrorMessage(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'This field is required';
    }
    if (field?.hasError('email')) {
      return 'Enter a valid email';
    }
    if (field?.hasError('minlength')) {
      const requiredLength = field.getError('minlength').requiredLength;
      return `Minimum length is ${requiredLength} characters`;
    }
    return '';
  }
}
