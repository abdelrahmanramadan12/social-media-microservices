using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Service.Interfaces.RabbitMqServices;
namespace Service.Implementations.RabbitMQServices

{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {

        private readonly Lazy<Task<IConnection>> _lazyConn;
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(IConfiguration cfg)
        {
            _factory = new ConnectionFactory
            {
                //UserName = cfg.GetSection("RabbitQueues:Username").Value!,
                //Password = cfg.GetSection("RabbitQueues:Password").Value!,
                HostName = cfg.GetSection("RabbitQueues:HostName").Value!,
            };

            _lazyConn = new(() => _factory.CreateConnectionAsync());
        }

        public async Task PublishAsync<T>(T message, List<string> queueName, CancellationToken ct = default)
        {
            var conn = await _lazyConn.Value;
            await using var channel = await conn.CreateChannelAsync(
                new CreateChannelOptions(true, true), ct);

            foreach (var queue in queueName)
            {
                await channel.QueueDeclareAsync(queue, durable: true,
                                            exclusive: false, autoDelete: false,
                                            arguments: null, cancellationToken: ct);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                await channel.BasicPublishAsync(exchange: string.Empty,
                                                routingKey: queue,
                                                body: body,
                                                mandatory: true,
                                                basicProperties: new BasicProperties { Persistent = true },
                                                cancellationToken: ct);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_lazyConn.IsValueCreated)
                await (await _lazyConn.Value).DisposeAsync();
        }
    }
}