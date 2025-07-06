using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace UserManagement.Infrastructure.Services;

public class ExternalUserService(
    HttpClient httpClient,
    IUserRepository userRepository,
    IAvatarService avatarService,
    ILogger<ExternalUserService> logger,
    IOptions<ExternalApiSettings> settings
) : IExternalUserService
{
    public async Task<List<User>> ImportUsersFromExternalApiAsync()
    {
        try
        {
            var response = await httpClient.GetAsync(settings.Value.JsonPlaceholderUrl);
            response.EnsureSuccessStatusCode();

            var jsonUsers = await response.Content.ReadFromJsonAsync<List<JsonPlaceholderUser>>();
            if (jsonUsers == null || !jsonUsers.Any())
            {
                logger.LogWarning("No users received from JSONPlaceholder API");
                return new List<User>();
            }

            var maxRank = await userRepository.GetTotalCountAsync(null, null);
            var newUsers = new List<User>();

            foreach (var jsonUser in jsonUsers)
            {
                var existingUsers = await userRepository.GetUsersAsync(null, jsonUser.Email, 1, 1);

                if (existingUsers.Any())
                {
                    continue;
                }

                var user = new User(
                    jsonUser.Name,
                    jsonUser.Email,
                    jsonUser.Username,
                    avatarService.GenerateGravatarUrl(jsonUser.Email),
                    ++maxRank,
                    false,
                    DateTime.UtcNow
                );

                newUsers.Add(user);
            }

            if (newUsers.Any())
            {
                foreach (var user in newUsers)
                {
                    await userRepository.CreateUserAsync(user);
                }
            }

            logger.LogInformation("Imported {Count} users from JSONPlaceholder", newUsers.Count);
            
            return newUsers;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error importing users from JSONPlaceholder");
            throw;
        }
    }
}

public record JsonPlaceholderUser(
    int Id,
    string Name,
    string Email,
    string Username
); 