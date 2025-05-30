using System.Text;
using System.Text.Json;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Interfaces.PostServices;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class PostListener:IAsyncDisposable, IPostListener
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;
        private readonly IServiceScopeFactory _scopeFactory;

        public PostListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _userName = configuration.GetSection("PostAddedMQ:UserName").Value;
            _password = configuration.GetSection("PostAddedMQ:Password").Value;
            _hostName = configuration.GetSection("PostAddedMQ:HostName").Value;
            _queueName = configuration.GetSection("PostAddedMQ:QueueName").Value;
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
            if (_channel == null)
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
                    var postEvent = JsonSerializer.Deserialize<PostEvent>(message);
                    if (postEvent != null && postEvent.Data!=null &&!string.IsNullOrEmpty(postEvent.Data.PostId))
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {

                            // Call the service to handle the post deletion ==> delete all comments related to this post
                            var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
                            await postService.HandlePostEventAsync( postEvent);
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
