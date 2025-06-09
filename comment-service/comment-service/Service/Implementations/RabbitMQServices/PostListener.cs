using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Events;
using Service.Interfaces.PostServices;
using Service.Interfaces.RabbitMQServices;
using System.Text;
using System.Text.Json;

namespace Service.Implementations.RabbitMQServices
{
    public class PostListener : IAsyncDisposable, IPostListener, IHostedService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;
        private string _port;
        private readonly IServiceScopeFactory _scopeFactory;
        private CancellationTokenSource? _cancellationTokenSource;

        public PostListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _userName = configuration.GetSection("PostMQ:Username").Value;
            _password = configuration.GetSection("PostMQ:Password").Value;
            _hostName = configuration.GetSection("PostMQ:Hostname").Value;
            _queueName = configuration.GetSection("PostMQ:QueueName").Value;
            _port = configuration.GetSection("PostMQ:Port").Value;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostName,
                    Port = int.Parse(_port),
                    UserName = _userName,
                    Password = _password
                };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Channel is not initialized.");
            }

            try
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
                  try
                    {
                        var postEvent = JsonSerializer.Deserialize<PostEvent>(message);
                       if (postEvent != null && !string.IsNullOrEmpty(postEvent.PostId))
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
                                await postService.HandlePostEventAsync(postEvent);
                            }
                        }
                        else
                        {
                            Console.WriteLine("[PostListener] PostEvent is null or missing PostId.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PostListener] Failed to deserialize or handle PostEvent: {ex.Message}\n{ex.StackTrace}");
                    }
                    await Task.CompletedTask;
                };

                await _channel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: true,
                    consumerTag: "",
                    noLocal: false,
                    exclusive: false,
                    arguments: null,
                    consumer: consumer
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostListener] Error in ListenAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                await InitializeAsync();
                await ListenAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            await DisposeAsync();
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
            catch (Exception ex)
            {
                Console.WriteLine($"[PostListener] Error during DisposeAsync: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
