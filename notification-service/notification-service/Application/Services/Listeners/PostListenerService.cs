using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Listeners;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using Application.Interfaces;
using Infrastructure.Settings.RabbitMQ;
using RabbitMQ.Client;

namespace Application.Services.Listeners
{
    public class PostListenerService : IAsyncDisposable
    {
        private readonly RabbitMqListenerSettings _settings;
        private IConnection? _connection;
        private IModel? _channel;

        public PostListenerService(IOptions<RabbitMqListenerSettings> options)
        {
            _settings = options.Value;
        }

        public Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);

            return Task.CompletedTask;
        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[PostQueue] Message received: {message}");

                // TODO: Handle post message
            };

            _channel.BasicConsume(_settings.QueueName, true, consumer);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            return ValueTask.CompletedTask;
        }
    }

}