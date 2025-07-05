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
        var avatarUrl = avatarService.GenerateGravatarUrl(createUserDto.Email);
        var user = createUserDto.ToEntity(avatarUrl);

        var createdUser = await userRepository.CreateUserAsync(user);
        logger.LogInformation("Created user with ID: {Id}", createdUser.Id);
        return createdUser.ToDto();
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var existingUser = await userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
            return null;

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
        return users.Select(u => u.ToDto()).ToList();
    }
} 