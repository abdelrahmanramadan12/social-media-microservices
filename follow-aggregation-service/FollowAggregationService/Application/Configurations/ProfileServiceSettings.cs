using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configurations
{
    public class ProfileServiceSettings
    {
        public const string ConfigurationSection = "Services:Profile";
        public string BaseUrl { get; set; } = string.Empty;
    }
}
