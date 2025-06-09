using Microsoft.Extensions.Configuration;
using Service.Events;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentPublisher : ICommentPublisher
    {
        private readonly IRabbitMqPublisher _bus;
        private readonly List<string> _queueNames;

        public CommentPublisher(IRabbitMqPublisher bus, IConfiguration _config)
        {
            _bus = bus;
            _queueNames = _config["RabbitMQQueues:Comment"].Split(";").ToList();
        }
        public Task PublishAsync(CommentEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, _queueNames, ct);

    }
}
