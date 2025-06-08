using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configurations
{
    public class ReactionServiceSettings
    {
        public const string ConfigurationSection = "Services:Reaction";
        public string BaseUrl { get; set; } = string.Empty;
    }
}
