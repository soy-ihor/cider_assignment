namespace UserManagement.Application.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public string PolicyName { get; set; }
    public string AngularAppOrigin { get; set; }
} 