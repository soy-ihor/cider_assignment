using UserManagement.Infrastructure.DependencyInjection;
using UserManagement.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Add application services using extension methods
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddHostedServices()
    .AddCorsPolicy(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDevelopmentServices();
}

app.UseProductionServices()
   .UseSecurityServices()
   .UseEndpoints();

app.Run();
