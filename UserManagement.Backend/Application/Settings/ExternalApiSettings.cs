namespace UserManagement.Application.Settings;

public class ExternalApiSettings
{
    public const string SectionName = "ExternalApi";
    
    public string JsonPlaceholderUrl { get; set; } = "https://jsonplaceholder.typicode.com/users";
    public string GravatarBaseUrl { get; set; } = "https://www.gravatar.com/avatar";
} 