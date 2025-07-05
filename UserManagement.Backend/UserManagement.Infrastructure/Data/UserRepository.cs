using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Data;

public class UserRepository(
    ApplicationDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsersAsync(
        string? nameFilter,
        string? emailFilter,
        int pageNumber,
        int pageSize)
    {
        var query = context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(u => u.Name.Contains(nameFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            query = query.Where(u => u.Email.Contains(emailFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        var users = await query
            .OrderBy(u => u.Rank)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return users;
    }

    public async Task<int> GetTotalCountAsync(string? nameFilter, string? emailFilter)
    {
        var query = context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(u => u.Name.Contains(nameFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            query = query.Where(u => u.Email.Contains(emailFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        return await query.CountAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var maxRank = await context.Users
            .Where(u => !u.IsDeleted)
            .MaxAsync(u => (int?)u.Rank) ?? 0;

        user.Rank = maxRank + 1;
        user.CreatedAt = DateTime.UtcNow;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (existingUser == null)
        {
            return null;
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.Username = user.Username;
        existingUser.AvatarUrl = user.AvatarUrl;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null)
            return false;

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderUsersAsync(List<int> userIds)
    {
        var users = await context.Users
            .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
            .ToListAsync();

        if (users.Count != userIds.Count)
        {
            return false;
        }

        for (int i = 0; i < userIds.Count; i++)
        {
            var user = users.First(u => u.Id == userIds[i]);
            user.Rank = i + 1;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }
} 