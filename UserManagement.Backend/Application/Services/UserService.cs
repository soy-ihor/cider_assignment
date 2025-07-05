using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IExternalUserService externalUserService,
    IAvatarService avatarService,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<PaginatedResponseDto<UserDto>> GetUsersAsync(string? nameFilter, string? emailFilter, int pageNumber, int pageSize)
    {
        var users = await userRepository.GetUsersAsync(nameFilter, emailFilter, pageNumber, pageSize);
        var totalCount = await userRepository.GetTotalCountAsync(nameFilter, emailFilter);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var userDtos = users.Select(MapToDto).ToList();

        return new PaginatedResponseDto<UserDto>(
            userDtos,
            totalCount,
            pageNumber,
            pageSize,
            totalPages
        );
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User(
            createUserDto.Name,
            createUserDto.Email,
            createUserDto.Username,
            avatarService.GenerateGravatarUrl(createUserDto.Email),
            0,
            false,
            DateTime.UtcNow
        );

        var createdUser = await userRepository.CreateUserAsync(user);
        logger.LogInformation("Created user with ID: {Id}", createdUser.Id);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var existingUser = await userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
            return null;

        var oldEmail = existingUser.Email;
        existingUser.Name = updateUserDto.Name;
        existingUser.Email = updateUserDto.Email;
        existingUser.Username = updateUserDto.Username;
        if (oldEmail != updateUserDto.Email)
        {
            existingUser.AvatarUrl = avatarService.GenerateGravatarUrl(updateUserDto.Email);
        }
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await userRepository.UpdateUserAsync(id, existingUser);
        if (updatedUser != null)
        {
            logger.LogInformation("Updated user with ID: {Id}", id);
        }
        return updatedUser != null ? MapToDto(updatedUser) : null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var result = await userRepository.DeleteUserAsync(id);
        if (result)
        {
            logger.LogInformation("Deleted user with ID: {Id}", id);
        }
        return result;
    }

    public async Task<bool> ReorderUsersAsync(List<int> userIds)
    {
        var result = await userRepository.ReorderUsersAsync(userIds);
        if (result)
        {
            logger.LogInformation("Reordered users");
        }
        return result;
    }

    public async Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync()
    {
        var users = await externalUserService.ImportUsersFromExternalApiAsync();
        logger.LogInformation("Imported {Count} users from JSONPlaceholder", users.Count);
        return users.Select(MapToDto).ToList();
    }

    private static UserDto MapToDto(User user) =>
        new(
            user.Id,
            user.Name,
            user.Email,
            user.Username,
            user.AvatarUrl,
            user.Rank,
            user.CreatedAt,
            user.UpdatedAt
        );
} 