using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Domain.RabbitSetting
{
    public  class RabbitSetting
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string QueueName { get; set; } 

    }
}
