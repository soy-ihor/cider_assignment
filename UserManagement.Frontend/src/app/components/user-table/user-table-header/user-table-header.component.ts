import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-user-table-header',
  templateUrl: './user-table-header.component.html',
  styleUrls: ['./user-table-header.component.scss'],
})
export class UserTableHeaderComponent {
  @Output() filterNameChange = new EventEmitter<string>();
  @Output() filterEmailChange = new EventEmitter<string>();
  @Output() importUsers = new EventEmitter<void>();
  @Output() addUser = new EventEmitter<void>();

  filterName = new FormControl('');
  filterEmail = new FormControl('');
  loading = false;

  constructor() {
    this.filterName.valueChanges.subscribe((value) => {
      this.filterNameChange.emit(value || '');
    });

    this.filterEmail.valueChanges.subscribe((value) => {
      this.filterEmailChange.emit(value || '');
    });
  }

  onImportUsers(): void {
    this.importUsers.emit();
  }

  onAddUser(): void {
    this.addUser.emit();
  }
}
