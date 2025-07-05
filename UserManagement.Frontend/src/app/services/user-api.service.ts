import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  User,
  CreateUserDto,
  UpdateUserDto,
  PaginatedResponse,
  ReorderUsersDto,
} from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly apiUrl = `${environment.apiBaseUrl}/users`;

  constructor(private readonly http: HttpClient) {}

  getUsers(filter: {
    name?: string;
    email?: string;
    page?: number;
    pageSize?: number;
  }): Observable<PaginatedResponse<User>> {
    let params = new HttpParams();
    if (filter.name) params = params.set('name', filter.name);
    if (filter.email) params = params.set('email', filter.email);
    if (filter.page) params = params.set('page', filter.page);
    if (filter.pageSize) params = params.set('pageSize', filter.pageSize);
    return this.http.get<PaginatedResponse<User>>(this.apiUrl, { params });
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(dto: CreateUserDto): Observable<User> {
    return this.http.post<User>(this.apiUrl, dto);
  }

  updateUser(id: number, dto: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, dto);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  importUsers(): Observable<User[]> {
    return this.http.post<User[]>(`${this.apiUrl}/generate`, {});
  }

  reorderUsers(dto: ReorderUsersDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/reorder`, dto);
  }
}
