using Microsoft.Extensions.Options;
using UserManagement.Application.Settings;

namespace UserManagement.Infrastructure.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDevelopmentServices(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        return app;
    }

    public static IApplicationBuilder UseProductionServices(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        
        return app;
    }

    public static IApplicationBuilder UseSecurityServices(this IApplicationBuilder app)
    {
        var corsSettings = app.ApplicationServices.GetRequiredService<IOptions<CorsSettings>>().Value;

        if (corsSettings is not null)
        {
            app.UseCors(corsSettings.PolicyName);
        }

        app.UseAuthorization();
        
        return app;
    }

    public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        return app;
    }
} 