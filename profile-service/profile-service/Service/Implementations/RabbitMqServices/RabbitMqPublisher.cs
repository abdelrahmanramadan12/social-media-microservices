using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Service.Interfaces.RabbitMqServices;
namespace Service.Implementations.RabbitMQServices

{
    public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
    {

        private readonly Lazy<Task<IConnection>> _lazyConn;
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(IConfiguration cfg)
        {
            _factory = new ConnectionFactory
            {
                HostName = cfg["RabbitMQ:Host"] ?? "localhost",
                //UserName = cfg["RabbitMQ:User"],
                //Password = cfg["RabbitMQ:Pass"],
            };

            _lazyConn = new(() => _factory.CreateConnectionAsync());
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default)
        {
            var conn = await _lazyConn.Value;
            await using var channel = await conn.CreateChannelAsync(
                new CreateChannelOptions(true, true), ct);

            await channel.QueueDeclareAsync(queueName, durable: true,
                                            exclusive: false, autoDelete: false,
                                            arguments: null, cancellationToken: ct);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(exchange: string.Empty,
                                            routingKey: queueName,
                                            body: body,
                                            mandatory: true,
                                            basicProperties: new BasicProperties { Persistent = true },
                                            cancellationToken: ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_lazyConn.IsValueCreated)
                await (await _lazyConn.Value).DisposeAsync();
        }
    }
}