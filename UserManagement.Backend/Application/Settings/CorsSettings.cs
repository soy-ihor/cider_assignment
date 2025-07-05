namespace UserManagement.Application.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";
    
    public string PolicyName { get; set; } = "AllowAngularApp";
    public string AngularAppOrigin { get; set; } = "http://localhost:4200";
} 