import { Component, EventEmitter, Output } from '@angular/core';
import { UserApiService } from '../../services/user-api.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-import-users',
  templateUrl: './import-users.component.html',
  styleUrls: ['./import-users.component.scss'],
})
export class ImportUsersComponent {
  loading = false;
  @Output() usersImported = new EventEmitter<User[]>();

  constructor(private userApi: UserApiService) {}

  importUsers(): void {
    this.loading = true;
    this.userApi.importUsers().subscribe({
      next: (users) => {
        this.loading = false;
        this.usersImported.emit(users);
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
