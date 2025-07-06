namespace UserManagement.Domain.Entities
{
    public class User(
        string name,
        string email,
        string username,
        string avatarUrl,
        int rank,
        bool isDeleted,
        DateTime createdAt,
        DateTime? updatedAt = null)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
        public string Username { get; set; } = username;
        public string AvatarUrl { get; set; } = avatarUrl;
        public int Rank { get; set; } = rank;
        public bool IsDeleted { get; set; } = isDeleted;
        public DateTime CreatedAt { get; set; } = createdAt;
        public DateTime? UpdatedAt { get; set; } = updatedAt;
    }
} 