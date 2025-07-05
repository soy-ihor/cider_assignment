using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Data;

public class DbSeeder(ApplicationDbContext context) : IDbSeeder
{
    public async Task SeedAsync()
    {
        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User(
                    "John Doe",
                    "john.doe@example.com",
                    "johndoe",
                    "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50?d=identicon&s=80",
                    1,
                    false,
                    DateTime.UtcNow
                ),
                new User(
                    "Jane Smith",
                    "jane.smith@example.com",
                    "janesmith",
                    "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d51?d=identicon&s=80",
                    2,
                    false,
                    DateTime.UtcNow
                ),
                new User(
                    "Bob Johnson",
                    "bob.johnson@example.com",
                    "bobjohnson",
                    "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d52?d=identicon&s=80",
                    3,
                    false,
                    DateTime.UtcNow
                ),
                new User(
                    "Alice Brown",
                    "alice.brown@example.com",
                    "alicebrown",
                    "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d53?d=identicon&s=80",
                    4,
                    false,
                    DateTime.UtcNow
                ),
                new User(
                    "Charlie Wilson",
                    "charlie.wilson@example.com",
                    "charliewilson",
                    "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d54?d=identicon&s=80",
                    5,
                    false,
                    DateTime.UtcNow
                )
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
} 