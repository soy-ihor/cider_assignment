using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Application.Settings;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Services;

namespace UserManagement.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAvatarService, AvatarService>();
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure settings
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<CorsSettings>(
            configuration.GetSection(CorsSettings.SectionName));
        services.Configure<ExternalApiSettings>(
            configuration.GetSection(ExternalApiSettings.SectionName));
        
        // Database
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseInMemoryDatabase(databaseSettings?.InMemoryDatabaseName ?? "UserManagementDb"));
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        // External services
        services.AddScoped<IExternalUserService, ExternalUserService>();
        
        // Database seeding
        services.AddScoped<IDbSeeder, DbSeeder>();
        
        return services;
    }

    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseSeederHostedService>();
        
        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
        
        services.AddCors(options =>
        {
            options.AddPolicy(corsSettings?.PolicyName ?? "AllowAngularApp",
                policy =>
                {
                    policy.WithOrigins(corsSettings?.AngularAppOrigin ?? "http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });
        
        return services;
    }
} 