using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Configuration
{
    public class MediaServiceSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }
}
