using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using react_service.Application.Events;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Repositories;
using react_service.Domain.Entites;
using System.Text;
using System.Text.Json;

namespace Workers
{
    public class CommentEventListener : ICommentEventListner
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly string _queueName;
        private IConnection? _connection;
        private IChannel? _channel;

        public CommentEventListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;

            _username = _configuration.GetSection("RabbitQueues:Username").Value!;
            _password = _configuration.GetSection("RabbitQueues:Password").Value!;
            _hostname = _configuration.GetSection("RabbitQueues:HostName").Value!;
            _port = Convert.ToInt32(_configuration.GetSection("RabbitQueues:Port").Value);
            _queueName = _configuration.GetSection("RabbitQueues:CommentQueue").Value!;

            if (string.IsNullOrEmpty(_hostname) || string.IsNullOrEmpty(_username) ||
                string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_queueName))
            {
                throw new InvalidOperationException("RabbitMQ:CommentEvent configuration is incomplete. Please check appsettings.json");
            }
        }

        public async Task InitializeAsync()
        {
            try
            {

                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Consumer not initialized.");
            }


            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    // Log event receipt
                    using var scope = _scopeFactory.CreateScope();
                    var commentRepository = scope.ServiceProvider.GetRequiredService<ICommentRepository>();

                    var commentEvent = JsonSerializer.Deserialize<CommentEvent>(messageJson);
                    if (commentEvent != null)
                    {

                        switch (commentEvent.EventType)
                        {
                            case EventType.Create:
                                var comment = new Comment
                                {
                                    CommentId = commentEvent.CommentId,
                                    AuthorId = commentEvent.CommentAuthorId,
                                    IsDeleted = false
                                };
                                var result = await commentRepository.AddComment(comment);
                                break;

                            case EventType.Delete:
                                var deleteResult = await commentRepository.DeleteComment(commentEvent.CommentId);
                                break;

                            default:
                                break;
                        }

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {

            try
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                }
                if (_connection != null)
                {
                    await _connection.CloseAsync();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
