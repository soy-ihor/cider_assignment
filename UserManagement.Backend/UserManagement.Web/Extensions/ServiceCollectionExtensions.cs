using Microsoft.EntityFrameworkCore;
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
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<CorsSettings>(
            configuration.GetSection(CorsSettings.SectionName));
        services.Configure<ExternalApiSettings>(
            configuration.GetSection(ExternalApiSettings.SectionName));
        
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseInMemoryDatabase(databaseSettings?.InMemoryDatabaseName ?? "UserManagementDb"));
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IExternalUserService, ExternalUserService>();
        
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

        if (corsSettings != null)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(corsSettings.PolicyName,
                policy =>
                {
                    policy.WithOrigins(corsSettings.AngularAppOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        return services;
    }
} 