using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserManagement.API.Application.DTOs;
using UserManagement.API.Domain.Entities;
using UserManagement.API.Domain.Interfaces;
using UserManagement.API.Infrastructure.External;

namespace UserManagement.API.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, HttpClient httpClient, ILogger<UserRepository> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserDto>> GetUsersAsync(string? nameFilter, string? emailFilter, int pageNumber, int pageSize)
        {
            var query = _context.Users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                var nameFilterLower = nameFilter.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(nameFilterLower));
            }

            if (!string.IsNullOrWhiteSpace(emailFilter))
            {
                var emailFilterLower = emailFilter.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(emailFilterLower));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var users = await query
                .OrderBy(u => u.Rank)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Username = u.Username,
                    AvatarUrl = u.AvatarUrl,
                    Rank = u.Rank,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResponseDto<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Username = u.Username,
                    AvatarUrl = u.AvatarUrl,
                    Rank = u.Rank,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var maxRank = await _context.Users
                .Where(u => !u.IsDeleted)
                .MaxAsync(u => (int?)u.Rank) ?? 0;

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Username = createUserDto.Username,
                AvatarUrl = GenerateGravatarUrl(createUserDto.Email),
                Rank = maxRank + 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Rank = user.Rank,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                return null;

            var oldEmail = user.Email;
            
            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;
            user.Username = updateUserDto.Username;
            
            // Only regenerate avatar if email changed
            if (oldEmail != updateUserDto.Email)
            {
                user.AvatarUrl = GenerateGravatarUrl(updateUserDto.Email);
            }
            
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Rank = user.Rank,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderUsersAsync(List<int> userIds)
        {
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();

            if (users.Count != userIds.Count)
                return false;

            for (int i = 0; i < userIds.Count; i++)
            {
                var user = users.First(u => u.Id == userIds[i]);
                user.Rank = i + 1;
                user.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> ImportUsersFromJsonPlaceholderAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
                response.EnsureSuccessStatusCode();

                var jsonUsers = await response.Content.ReadFromJsonAsync<List<JsonPlaceholderUser>>();
                if (jsonUsers == null || !jsonUsers.Any())
                {
                    _logger.LogWarning("No users received from JSONPlaceholder API");
                    return new List<UserDto>();
                }

                var maxRank = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .MaxAsync(u => (int?)u.Rank) ?? 0;

                var newUsers = new List<User>();
                var importedUsers = new List<UserDto>();

                foreach (var jsonUser in jsonUsers)
                {
                    // Check if user already exists
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == jsonUser.Email && !u.IsDeleted);

                    if (existingUser != null)
                        continue;

                    var user = new User
                    {
                        Name = jsonUser.Name,
                        Email = jsonUser.Email,
                        Username = jsonUser.Username,
                        AvatarUrl = GenerateGravatarUrl(jsonUser.Email),
                        Rank = ++maxRank,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    newUsers.Add(user);
                }

                if (newUsers.Any())
                {
                    _context.Users.AddRange(newUsers);
                    await _context.SaveChangesAsync();

                    importedUsers = newUsers.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Username = u.Username,
                        AvatarUrl = u.AvatarUrl,
                        Rank = u.Rank,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    }).ToList();
                }

                _logger.LogInformation("Imported {Count} users from JSONPlaceholder", importedUsers.Count);
                return importedUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing users from JSONPlaceholder");
                throw;
            }
        }

        private static string GenerateGravatarUrl(string email)
        {
            using var md5 = MD5.Create();
            var emailBytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
            var hashBytes = md5.ComputeHash(emailBytes);
            var hash = Convert.ToHexString(hashBytes).ToLower();
            return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s=80";
        }
    }
} 