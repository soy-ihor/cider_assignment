export interface User {
  id: number;
  name: string;
  email: string;
  username: string;
  avatarUrl: string;
  rank: number;
}

export interface CreateUserDto {
  name: string;
  email: string;
  username: string;
}

export interface UpdateUserDto {
  name: string;
  email: string;
  username: string;
}

export interface ReorderUsersDto {
  userIds: number[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
