using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configurations
{
    public class FollowServiceSettings
    {
        public const string ConfigurationSection = "Services:Follow";
        public string BaseUrl { get; set; } = string.Empty;
    }
}
