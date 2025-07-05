import { Component, EventEmitter, Output, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserApiService } from '../../services/user-api.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-import-users',
  templateUrl: './import-users.component.html',
})
export class ImportUsersComponent implements OnDestroy {
  loading = false;
  @Output() usersImported = new EventEmitter<User[]>();
  private destroy$ = new Subject<void>();

  constructor(private userApi: UserApiService) {}

  importUsers(): void {
    this.loading = true;
    this.userApi
      .importUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users) => {
          this.loading = false;
          this.usersImported.emit(users);
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
