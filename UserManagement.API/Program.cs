using Microsoft.EntityFrameworkCore;
using UserManagement.API.Infrastructure.Data;
using UserManagement.API.Domain.Interfaces;
using UserManagement.API.Application.Services;
using UserManagement.API.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core with In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("UserManagementDb"));

// Add HttpClient for external API calls
builder.Services.AddHttpClient();

// Add Repository and Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedDataAsync(context);
}

app.Run();

static async Task SeedDataAsync(ApplicationDbContext context)
{
    if (!context.Users.Any())
    {
        var users = new[]
        {
            new User
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Username = "johndoe",
                AvatarUrl = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50?d=identicon&s=80",
                Rank = 1,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Username = "janesmith",
                AvatarUrl = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d51?d=identicon&s=80",
                Rank = 2,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Bob Johnson",
                Email = "bob.johnson@example.com",
                Username = "bobjohnson",
                AvatarUrl = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d52?d=identicon&s=80",
                Rank = 3,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Alice Brown",
                Email = "alice.brown@example.com",
                Username = "alicebrown",
                AvatarUrl = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d53?d=identicon&s=80",
                Rank = 4,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Charlie Wilson",
                Email = "charlie.wilson@example.com",
                Username = "charliewilson",
                AvatarUrl = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d54?d=identicon&s=80",
                Rank = 5,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
