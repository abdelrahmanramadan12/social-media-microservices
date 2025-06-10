namespace Application.Configurations
{
    public class ProfileServiceSettings
    {
        public const string ConfigurationSection = "Services:Profile";
        public string BaseUrl { get; set; } = string.Empty;
    }
}
