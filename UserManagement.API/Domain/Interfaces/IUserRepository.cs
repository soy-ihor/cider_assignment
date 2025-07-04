using UserManagement.API.Domain.Entities;
using UserManagement.API.Application.DTOs;

namespace UserManagement.API.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<PaginatedResponseDto<UserDto>> GetUsersAsync(string? nameFilter, string? emailFilter, int pageNumber, int pageSize);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ReorderUsersAsync(List<int> userIds);
        Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync();
    }
} 