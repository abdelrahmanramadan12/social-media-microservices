using Application.Events;
using Domain.Enums;
using Domain.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Workers
{
    public class CommentEventConsumer : IHostedService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IConfiguration _configuration;
        private readonly IPostRepository _postRepository;
        private readonly string _queueName;

        public CommentEventConsumer(IConfiguration configuration, IPostRepository postRepository)
        {
            _configuration = configuration;
            _postRepository = postRepository;
            _queueName = _configuration.GetSection("CommentMQ:QueueName").Value!;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetSection("CommentMQ:HostName").Value!,
                UserName = _configuration.GetSection("CommentMQ:UserName").Value!,
                Password = _configuration.GetSection("CommentMQ:Password").Value!,
                Port = Convert.ToInt32(_configuration.GetSection("CommentMQ:Port").Value)
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var commentEvent = JsonSerializer.Deserialize<CommentEvent>(message);

                if (commentEvent != null)
                {
                    if (commentEvent.EventType == EventType.Create)
                    {
                        await _postRepository.UpdatePostCountersAsync(commentEvent.PostId, PostCounterEvent.CommentCreated);
                    }
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}