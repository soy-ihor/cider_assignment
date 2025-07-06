using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IExternalUserService
    {
        Task<List<User>> ImportUsersFromExternalApiAsync();
    }
} 