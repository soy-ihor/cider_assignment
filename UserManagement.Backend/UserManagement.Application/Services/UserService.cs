using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
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
        if (pageNumber < 1) 
        {
            pageNumber = 1;
        } 

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 10;
        } 

        var users = await userRepository.GetUsersAsync(nameFilter, emailFilter, pageNumber, pageSize);
        var totalCount = await userRepository.GetTotalCountAsync(nameFilter, emailFilter);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var userDtos = users.Select(u => u.ToDto()).ToList();

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
        return user?.ToDto();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (createUserDto == null)
        {
            logger.LogWarning("CreateUserAsync called with null DTO");
            throw new ArgumentNullException(nameof(createUserDto));
        }

        var existingUser = await userRepository.GetUserByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            logger.LogWarning("Attempted to create user with existing email: {Email}", createUserDto.Email);
            throw new InvalidOperationException($"User with email {createUserDto.Email} already exists");
        }

        var avatarUrl = avatarService.GenerateGravatarUrl(createUserDto.Email);
        var user = createUserDto.ToEntity(avatarUrl);

        var createdUser = await userRepository.CreateUserAsync(user);
        logger.LogInformation($"Created user with ID: {createdUser.Id}");
        return createdUser.ToDto();
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        if (updateUserDto == null)
        {
            logger.LogWarning("UpdateUserAsync called with null DTO for user ID: {Id}", id);
            throw new ArgumentNullException(nameof(updateUserDto));
        }

        var existingUser = await userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            logger.LogWarning("Attempted to update non-existent user with ID: {Id}", id);
            return null;
        }

        if (existingUser.Email != updateUserDto.Email)
        {
            var userWithNewEmail = await userRepository.GetUserByEmailAsync(updateUserDto.Email);
            if (userWithNewEmail != null && userWithNewEmail.Id != id)
            {
                logger.LogWarning("Attempted to update user {Id} with existing email: {Email}", id, updateUserDto.Email);
                throw new InvalidOperationException($"User with email {updateUserDto.Email} already exists");
            }
        }

        var oldEmail = existingUser.Email;
        var avatarUrl = oldEmail != updateUserDto.Email 
            ? avatarService.GenerateGravatarUrl(updateUserDto.Email)
            : existingUser.AvatarUrl;

        var updatedUser = updateUserDto.ToEntity(existingUser, avatarUrl);
        var result = await userRepository.UpdateUserAsync(id, updatedUser);
        
        if (result != null)
        {
            logger.LogInformation("Updated user with ID: {Id}", id);
        }
        return result?.ToDto();
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
        if (userIds == null || userIds.Count == 0)
        {
            logger.LogWarning("ReorderUsersAsync called with null or empty user IDs list");
            return false;
        }

        foreach (var userId in userIds)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("Invalid user ID provided for reordering: {UserId}", userId);
                return false;
            }
        }

        var result = await userRepository.ReorderUsersAsync(userIds);
        if (result)
        {
            logger.LogInformation("Reordered {Count} users", userIds.Count);
        }
        return result;
    }

    public async Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync()
    {
        var users = await externalUserService.ImportUsersFromExternalApiAsync();
        logger.LogInformation("Imported {Count} users from JSONPlaceholder", users.Count);
        return users.Select(u => u.ToDto()).ToList();
    }
} 