//using System.Text;
//using System.Text.Json;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
////using Service.Interfaces.FollowServices;

//namespace Domain.RabbitMQ
//{
//    public class FollowListener : IFollowListener, IAsyncDisposable
//    {
//        private IConnection? _connection;
//        private IChannel? _channel;
//        private string _userName;
//        private string _password;
//        private string _hostName;
//        private string _queueName;
//        private readonly IServiceScopeFactory _scopeFactory;

//        public FollowListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
//        {
//            _scopeFactory = scopeFactory;
//            _userName = configuration["FollowMQ:UserName"];
//            _password = configuration["FollowMQ:Password"];
//            _hostName = configuration["FollowMQ:HostName"];
//            _queueName = configuration["FollowMQ:QueueName"];
//        }
//        public async Task InitializeAsync()
//        {
//            var factory = new ConnectionFactory()
//            {
//                HostName = _hostName,
//                //UserName = _userName,
//                //Password = _password,
//            };
//            _connection = await factory.CreateConnectionAsync();
//            _channel = await _connection.CreateChannelAsync();
//        }

//        public async Task ListenAsync(CancellationToken cancellationToken)
//        {
//            if (_channel == null)
//            {
//                throw new InvalidOperationException("Channel is not initialized.");
//            }

//            await _channel.QueueDeclareAsync(queue: _queueName,
//                    durable: false,
//                    exclusive: false,
//                    autoDelete: false,
//                    arguments: null);
//            var consumer = new AsyncEventingBasicConsumer(_channel);
//            consumer.ReceivedAsync += async (model, ea) =>
//            {
//                try
//                {
//                    var body = ea.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);
//                    // Deserialize the message
//                    var followEvent = JsonSerializer.Deserialize<FollowEvent>(message);
//                    if (followEvent != null && !string.IsNullOrEmpty(followEvent.FollowerId) && !string.IsNullOrEmpty(followEvent.FollowingId))
//                    {
//                        using (var scope = _scopeFactory.CreateScope())
//                        {

//                            // Call the service to handle the Follow Event
//                            var followService = scope.ServiceProvider.GetRequiredService<IFollowCounterService>();
//                            // Update the follow counter based on the event type
//                            await followService.UpdateCounter(followEvent.FollowerId, followEvent.FollowingId, followEvent.EventType);
//                        }
//                    }

//                }
//                catch (Exception ex)
//                {
//                    // Handle the exception
//                    Console.WriteLine($"Error processing message: {ex.Message}");
//                }
//            };
//            await _channel.BasicConsumeAsync(
//                queue: _queueName,
//                autoAck: true,
//                consumer: consumer,
//                cancellationToken: cancellationToken);

//        }

//        public async ValueTask DisposeAsync()
//        {

//            if (_channel != null)
//            {
//                await _channel.CloseAsync();
//            }
//            if (_connection != null)
//            {
//                await _connection.CloseAsync();
//            }
//        }

//    }
//}