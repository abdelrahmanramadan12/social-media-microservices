using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Redis
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
    }
}
