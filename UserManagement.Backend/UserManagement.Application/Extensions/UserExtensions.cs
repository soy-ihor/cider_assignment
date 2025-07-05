using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Extensions;

public static class UserExtensions
{
    public static UserDto ToDto(this User user) =>
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

    public static User ToEntity(this CreateUserDto dto, string avatarUrl) =>
        new(
            dto.Name,
            dto.Email,
            dto.Username,
            avatarUrl,
            0,
            false,
            DateTime.UtcNow
        );

    public static User ToEntity(this UpdateUserDto dto, User existingUser, string avatarUrl)
    {
        existingUser.Name = dto.Name;
        existingUser.Email = dto.Email;
        existingUser.Username = dto.Username;
        existingUser.AvatarUrl = avatarUrl;
        existingUser.UpdatedAt = DateTime.UtcNow;
        
        return existingUser;
    }
} 