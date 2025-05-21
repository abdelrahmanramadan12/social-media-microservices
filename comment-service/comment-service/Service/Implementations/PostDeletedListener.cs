using System.Text;
using System.Text.Json;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Interfaces;

namespace Service.Implementations
{
    public class PostDeletedListener : IPostDeletedListener
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;
        private readonly IServiceScopeFactory _scopeFactory;

        public PostDeletedListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _userName = configuration.GetSection("ProfileDeletedMQ:UserName").Value;
            _password = configuration.GetSection("ProfileDeletedMQ:Password").Value;
            _hostName = configuration.GetSection("ProfileDeletedMQ:HostName").Value;
            _queueName = configuration.GetSection("ProfileDeletedMQ:QueueName").Value;
        }
        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                //UserName = _userName,
                //Password = _password,
            };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            if ( _channel == null)
            {
                throw new InvalidOperationException("Channel is not initialized.");
            }

            await _channel.QueueDeclareAsync(queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    // Deserialize the message
                    var postDeletedEvent = JsonSerializer.Deserialize<PostDeletedEvent>(message);
                    if (postDeletedEvent != null && !string.IsNullOrEmpty(postDeletedEvent.PostId))
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {

                            // Call the service to handle the post deletion ==> delete all comments related to this post
                            var postDeletedService = scope.ServiceProvider.GetRequiredService<IPostDeletedService>();
                            await postDeletedService.HandlePostDeletedAsync(postDeletedEvent.PostId);
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Handle the exception
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };
            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: cancellationToken);

        }

        public async ValueTask DisposeAsync()
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
    }
}
