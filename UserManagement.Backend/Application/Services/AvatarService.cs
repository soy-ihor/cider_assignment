using System.Security.Cryptography;
using System.Text;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Settings;
using Microsoft.Extensions.Options;

namespace UserManagement.Application.Services;

public class AvatarService(IOptions<ExternalApiSettings> settings) : IAvatarService
{
    public string GenerateGravatarUrl(string email)
    {
        using var md5 = MD5.Create();
        var emailBytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
        var hashBytes = md5.ComputeHash(emailBytes);
        var hash = Convert.ToHexString(hashBytes).ToLower();
        return $"{settings.Value.GravatarBaseUrl}/{hash}?d=identicon&s=80";
    }
} 