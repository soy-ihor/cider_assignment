using UserManagement.API.Application.DTOs;
using UserManagement.API.Domain.Interfaces;

namespace UserManagement.API.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserDto>> GetUsersAsync(string? nameFilter, string? emailFilter, int pageNumber, int pageSize)
        {
            var result = await _userRepository.GetUsersAsync(nameFilter, emailFilter, pageNumber, pageSize);
            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = await _userRepository.CreateUserAsync(createUserDto);
            _logger.LogInformation("Created user with ID: {Id}", user.Id);
            return user;
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.UpdateUserAsync(id, updateUserDto);
            
            if (user != null)
            {
                _logger.LogInformation("Updated user with ID: {Id}", id);
            }
            
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
            
            if (result)
            {
                _logger.LogInformation("Deleted user with ID: {Id}", id);
            }
            
            return result;
        }

        public async Task<bool> ReorderUsersAsync(List<int> userIds)
        {
            var result = await _userRepository.ReorderUsersAsync(userIds);

            if (result)
            {
                _logger.LogInformation("Reordered users");
            }

            return result;
        }

        public async Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync()
        {
            var users = await _userRepository.ImportUsersFromJsonPlaceholderAsync();
            _logger.LogInformation("Imported {Count} users from JSONPlaceholder", users.Count);
            return users;
        }
    }
} 