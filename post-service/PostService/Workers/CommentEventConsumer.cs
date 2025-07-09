using Application.Events;
using Application.Interfaces;
using Domain.Enums;
using Domain.IRepository;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Workers
{
    public class CommentEventConsumer : IQueueListener<CommentEvent>
    {
        private IConnection _connection;
        private IChannel _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private int _port;
        private readonly IPostRepository _postRepository;
        private readonly string _queueName;

        public CommentEventConsumer(IConfiguration _configuration, IPostRepository postRepository)
        {
            _userName = _configuration.GetSection("RabbitQueues:Username").Value!;
            _password = _configuration.GetSection("RabbitQueues:Password").Value!;
            _hostName = _configuration.GetSection("RabbitQueues:HostName").Value!;
            _port = Convert.ToInt32(_configuration.GetSection("RabbitQueues:Port").Value);
            _queueName = _configuration.GetSection("RabbitQueues:CommentQueue").Value!;
            _postRepository = postRepository;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                Port = _port
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task ListenAsync(CancellationToken _cancellationToken)
        {
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
                    } else if (commentEvent.EventType == EventType.Delete)
                    {
                        await _postRepository.UpdatePostCountersAsync(commentEvent.PostId, PostCounterEvent.CommentDeleted);
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

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}