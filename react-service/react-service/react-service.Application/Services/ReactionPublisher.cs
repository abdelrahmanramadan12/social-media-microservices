
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.DTO.RabbitMQ;
using System.Text.Json;
using System.Text;
using react_service.Domain.Events;

namespace react_service.Application.Services
{
    public class ReactionPublisher : IReactionPublisher, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly List<string> _queueNames;
        private readonly string _hostnameNotif;
        private readonly string _usernameNotif;
        private readonly string _passwordNotif;
        private IConnection? _connection;
        private IChannel? _channel;
        private IConnection _connectionNotif;
        private IChannel _channelNotif;
        private string? _queueNameNotif;

        public ReactionPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            // Get RabbitMQ configuration
            _hostname = _configuration["RabbitMQ:Reaction:HostName"] ?? "localhost";
            _username = _configuration["RabbitMQ:Reaction:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:Reaction:Password"] ?? "guest";
            var queueNamesStr = _configuration["RabbitMQ:Reaction:QueueName"] ?? "ReactionQueue";
            _queueNames = queueNamesStr.Split(';').ToList();
            _hostnameNotif = _configuration["RabbitMQ:ReactionNotif:HostName"] ?? "localhost";
            _usernameNotif = _configuration["RabbitMQ:ReactionNotif:UserName"] ?? "guest";
            _passwordNotif = _configuration["RabbitMQ:ReactionNotif:Password"] ?? "guest";
            _queueNameNotif = _configuration["RabbitMQ:ReactionNotif:QueueName"];

            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        private async Task InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                var factorynotif = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                _connectionNotif = await factorynotif.CreateConnectionAsync();
                _channelNotif = await _connectionNotif.CreateChannelAsync();


                foreach (var queueName in _queueNames)
                {
                    await _channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                }

                await _channelNotif.QueueDeclareAsync(
                    queue: _queueNameNotif,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);   
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PublishPostReactionAsync(PostReactionEventDTO reactionEvent)
        {
            try
            {
                if (_channel == null)
                {
                    return;
                }

                var message = JsonSerializer.Serialize(reactionEvent);
                var body = Encoding.UTF8.GetBytes(message);

                foreach (var queueName in _queueNames)
                {
                    await _channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: new BasicProperties
                        {
                            Persistent = true
                        },
                        body: body);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task PublishReactionNotifAsync(ReactionEvent reactionEvent)
        {
            try
            {
                if (_channelNotif == null)
                {
                    return;
                }

                var message = JsonSerializer.Serialize(reactionEvent);
                var body = Encoding.UTF8.GetBytes(message);

             
                    await _channelNotif.BasicPublishAsync(
                        exchange: "",
                        routingKey: _queueNameNotif,
                        mandatory: true,
                        basicProperties: new BasicProperties
                        {
                            Persistent = true
                        },
                        body: body);

                
            }
            catch (Exception ex)
            {
                throw;
            }
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

                if (_channelNotif != null)
                {
                    await _channelNotif.CloseAsync();
                }
                if (_connectionNotif != null)
                {
                    await _connectionNotif.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PublishCommentReactionAsync(CommentReactionEventDTO reactionEvent)
        {
            try
            {
                if (_channel == null)
                {
                    return;
                }

                var message = JsonSerializer.Serialize(reactionEvent);
                var body = Encoding.UTF8.GetBytes(message);

                foreach (var queueName in _queueNames)
                {
                    await _channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: new BasicProperties
                        {
                            Persistent = true
                        },
                        body: body);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
