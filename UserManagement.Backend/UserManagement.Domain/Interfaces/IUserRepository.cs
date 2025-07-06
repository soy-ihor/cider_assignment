using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(string? nameFilter, string? emailFilter, int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync(string? nameFilter, string? emailFilter);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ReorderUsersAsync(List<int> userIds);
    }
} 