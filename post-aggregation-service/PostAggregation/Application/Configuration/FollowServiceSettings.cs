namespace Application.Configuration
{
    public class FollowServiceSettings
    {
        public const string ConfigurationSection = "Services:Follow";
        public string BaseUrl { get; set; } = string.Empty;
    }
} 