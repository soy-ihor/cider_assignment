using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedResponseDto<UserDto>> GetUsersAsync(
            string? nameFilter, string? emailFilter, int pageNumber, int pageSize);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ReorderUsersAsync(List<int> userIds);
        Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync();
    }
} 