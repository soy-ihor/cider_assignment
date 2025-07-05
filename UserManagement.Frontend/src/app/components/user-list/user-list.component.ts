import { Component, OnInit, ViewChild } from '@angular/core';
import { UserApiService } from '../../services/user-api.service';
import { User, PaginatedResponse } from '../../models/user.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  UserFormComponent,
  UserFormData,
} from '../user-form/user-form.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
})
export class UserListComponent implements OnInit {
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

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  setupFilters(): void {
    this.filterName.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.pageIndex = 0;
      this.loadUsers();
    });

    this.filterEmail.valueChanges.pipe(debounceTime(300)).subscribe(() => {
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
      .subscribe({
        next: (response: PaginatedResponse<User>) => {
          this.dataSource.data = response.items;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: () => {
          this.showMessage('Error loading users', 'error');
          this.loading = false;
        },
      });
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

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  editUser(user: User): void {
    this.openUserForm(user);
  }

  deleteUser(userId: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userApi.deleteUser(userId).subscribe({
        next: () => {
          this.showMessage('User deleted successfully', 'success');
          this.loadUsers();
        },
        error: () => {
          this.showMessage('Error deleting user', 'error');
        },
      });
    }
  }

  importUsers(): void {
    this.userApi.importUsers().subscribe({
      next: (users) => {
        this.showMessage(`Imported ${users.length} users`, 'success');
        this.loadUsers();
      },
      error: () => {
        this.showMessage('Error importing users', 'error');
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
    // Send new order to server
    this.userApi
      .reorderUsers({ userIds: this.dataSource.data.map((u) => u.id) })
      .subscribe({
        next: () => {
          this.showMessage('User order updated', 'success');
        },
        error: () => {
          // Revert order on error
          this.dataSource.data = prevData;
          this.dataSource._updateChangeSubscription();
          this.showMessage('Error updating user order', 'error');
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
