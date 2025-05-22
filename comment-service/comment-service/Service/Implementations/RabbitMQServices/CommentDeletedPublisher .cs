using Domain.Events;
using Microsoft.Extensions.Configuration;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentDeletedPublisher: ICommentDeletedPublisher
    {
        private readonly IRabbitMqPublisher _bus;
        private readonly string _queueName;

        public CommentDeletedPublisher(IRabbitMqPublisher bus, IConfiguration config)
        {
            _bus = bus;
            _queueName = config["RabbitMQQueues:CommentDeleted"];
        }

        public Task PublishAsync(CommentDeletedEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, _queueName, ct);
    }
}
