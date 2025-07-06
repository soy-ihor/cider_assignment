import { Component, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-user-table-header',
  templateUrl: './user-table-header.component.html',
  styleUrls: ['./user-table-header.component.scss'],
})
export class UserTableHeaderComponent implements OnDestroy {
  @Output() filterNameChange = new EventEmitter<string>();
  @Output() filterEmailChange = new EventEmitter<string>();
  @Output() importUsers = new EventEmitter<void>();
  @Output() addUser = new EventEmitter<void>();

  filterName = new FormControl('');
  filterEmail = new FormControl('');
  loading = false;
  private destroy$ = new Subject<void>();

  constructor() {
    this.filterName.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((value) => {
        this.filterNameChange.emit(value || '');
      });

    this.filterEmail.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((value) => {
        this.filterEmailChange.emit(value || '');
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onImportUsers(): void {
    this.importUsers.emit();
  }

  onAddUser(): void {
    this.addUser.emit();
  }
}
