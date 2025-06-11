using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces.RabbitMqServices
{
    public interface IRabbitMqPublisher : IAsyncDisposable
    {
        Task PublishAsync<T>(T message, List<string> queueName, CancellationToken ct = default);
    }
}
