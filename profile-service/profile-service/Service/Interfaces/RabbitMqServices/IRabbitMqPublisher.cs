using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces.RabbitMqServices
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default);
    }
}
