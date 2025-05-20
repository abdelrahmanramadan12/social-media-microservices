using Domain.DTOs;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Services.Implementations
{
    public class ProfileCreatedListener : IProfileCreatedListener
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private string _queueName;
        private readonly UserService _userService;

        public ProfileCreatedListener(IConfiguration config, UserService userService)
        {
            _userName = config.GetSection("ProfileCreatedMQ:UserName").Value;
            _password = config.GetSection("ProfileCreatedMQ:Password").Value;
            _hostName = config.GetSection("ProfileCreatedMQ:HostName").Value;
            _queueName = config.GetSection("ProfileCreatedMQ:QueueName").Value;
            _userService = userService;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                UserName = _userName,
                Password = _password,
                HostName = _hostName
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task ListenAsync()
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            await _channel.QueueDeclareAsync(_queueName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, args) =>
            {
                var messageJson = Encoding.UTF8.GetString(args.Body.ToArray());
                var userDto = JsonSerializer.Deserialize<UserDTO>(messageJson);

                if (userDto != null && !string.IsNullOrEmpty(userDto.UserId))
                {
                    await _userService.AddUser(userDto.UserId);
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
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
