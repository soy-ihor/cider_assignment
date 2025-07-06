namespace UserManagement.Application.Interfaces
{
    public interface IAvatarService
    {
        string GenerateGravatarUrl(string email);
    }
} 