using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {

        private readonly Lazy<Task<IConnection>> _lazyConn;
        private IChannel _channel;
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(IConfiguration cfg)
        {
            _factory = new ConnectionFactory
            {
                HostName = cfg["RabbitQueues:HostName"] ?? "localhost",
                //UserName = cfg["RabbitQueues:Username"],
                //Password = cfg["RabbitQueues:Password"],
            };

            _lazyConn = new(() => _factory.CreateConnectionAsync());
        }

        public async Task PublishAsync<T>(T message, List<string> queueNames, CancellationToken ct = default)
        {
            var conn = await _lazyConn.Value;
            
            if (_channel == null || !_channel.IsOpen)
            {
                _channel = await conn.CreateChannelAsync(
                new CreateChannelOptions(true, true), ct);
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            foreach (var queueName in queueNames)
            {
                await _channel.QueueDeclareAsync(
                    queueName, durable: true,
                    exclusive: false, autoDelete: false,
                    arguments: null, cancellationToken: ct
                );

                await _channel.BasicPublishAsync(exchange: string.Empty,
                    routingKey: queueName,
                    body: body,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    cancellationToken: ct
                );
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_lazyConn.IsValueCreated)
                await (await _lazyConn.Value).DisposeAsync();
        }
    }
}
