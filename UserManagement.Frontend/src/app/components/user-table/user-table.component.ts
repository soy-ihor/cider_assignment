import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { UserApiService } from '../../services/user-api.service';
import { User, PaginatedResponse } from '../../models/user.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  UserFormComponent,
  UserFormData,
} from '../user-form/user-form.component';

@Component({
  selector: 'app-user-table',
  templateUrl: './user-table.component.html',
  styleUrls: ['./user-table.component.scss'],
})
export class UserTableComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = [
    'rank',
    'avatar',
    'name',
    'email',
    'username',
    'actions',
  ];
  dataSource = new MatTableDataSource<User>([]);
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  loading = false;
  filterName = new FormControl('');
  filterEmail = new FormControl('');
  private destroy$ = new Subject<void>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private userApi: UserApiService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.setupFilters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  setupFilters(): void {
    this.filterName.valueChanges
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe(() => {
        this.pageIndex = 0;
        this.loadUsers();
      });

    this.filterEmail.valueChanges
      .pipe(debounceTime(300), takeUntil(this.destroy$))
      .subscribe(() => {
        this.pageIndex = 0;
        this.loadUsers();
      });
  }

  loadUsers(): void {
    this.loading = true;
    this.userApi
      .getUsers({
        name: this.filterName.value || undefined,
        email: this.filterEmail.value || undefined,
        page: this.pageIndex + 1,
        pageSize: this.pageSize,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: PaginatedResponse<User>) => {
          this.dataSource.data = response.items;
          this.totalCount = response.totalCount;
          this.loading = false;

          setTimeout(() => {
            if (this.paginator) {
              this.paginator.length = this.totalCount;
              this.paginator.pageIndex = this.pageIndex;
              this.paginator.pageSize = this.pageSize;
            }
          });
        },
      });
  }

  onFilterNameChange(value: string): void {
    this.filterName.setValue(value);
  }

  onFilterEmailChange(value: string): void {
    this.filterEmail.setValue(value);
  }

  onPageChange(event: any): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

  openUserForm(user?: User): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '600px',
      maxWidth: '90vw',
      data: user ? { user, isEdit: true } : { isEdit: false },
    });

    dialogRef
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((result) => {
        if (result) {
          if (!user) {
            this.pageIndex = 0;
          }
          this.loadUsers();
        }
      });
  }

  editUser(user: User): void {
    this.openUserForm(user);
  }

  deleteUser(userId: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userApi
        .deleteUser(userId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.showMessage('User deleted successfully', 'success');
            this.loadUsers();
          },
        });
    }
  }

  importUsers(): void {
    this.userApi
      .importUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users) => {
          this.showMessage(`Imported ${users.length} users`, 'success');
          this.loadUsers();
        },
      });
  }

  drop(event: CdkDragDrop<User[]>): void {
    const prevData = [...this.dataSource.data];

    moveItemInArray(
      this.dataSource.data,
      event.previousIndex,
      event.currentIndex
    );
    this.dataSource._updateChangeSubscription();

    this.userApi
      .reorderUsers({ userIds: this.dataSource.data.map((u) => u.id) })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.showMessage('User order updated', 'success');
        },
        error: () => {
          this.dataSource.data = prevData;
          this.dataSource._updateChangeSubscription();
        },
      });
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
