namespace UserManagement.Application.DTOs;

public record UserDto(
    int Id,
    string Name,
    string Email,
    string Username,
    string AvatarUrl,
    int Rank,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateUserDto(
    string Name,
    string Email,
    string Username
);

public record UpdateUserDto(
    string Name,
    string Email,
    string Username
);

public record ReorderUsersDto(
    List<int> UserIds
);

public record PaginatedResponseDto<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
); 