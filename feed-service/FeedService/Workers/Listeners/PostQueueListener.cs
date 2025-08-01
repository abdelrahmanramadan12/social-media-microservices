﻿using Application.Abstractions;
using Application.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Workers.Listeners
{
    public class PostQueueListener : IQueueListener<PostEvent>
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private int _port;
        private string _queueName;
        private readonly IFeedCommandService _feedCommandService;

        public PostQueueListener(IConfiguration config, IFeedCommandService feedCommandService)
        {
            _userName = config.GetSection("RabbitQueues:Username").Value!;
            _password = config.GetSection("RabbitQueues:Password").Value!;
            _hostName = config.GetSection("RabbitQueues:HostName").Value!;
            _queueName = config.GetSection("RabbitQueues:PostQueue").Value!;
            _port = Convert.ToInt32(config.GetSection("RabbitQueues:Port").Value);
            _feedCommandService = feedCommandService;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                UserName = _userName,
                Password = _password,
                HostName = _hostName,
                Port = _port
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task ListenAsync(CancellationToken _cancellationToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, args) =>
            {
                var messageJson = Encoding.UTF8.GetString(args.Body.ToArray());
                var postEvent = JsonSerializer.Deserialize<PostEvent>(messageJson);

                if (postEvent != null)
                {
                    if (postEvent.EventType == EventType.Create)
                    {
                        await _feedCommandService.PushToFeedsAsync(postEvent);
                    }
                    else if (postEvent.EventType == EventType.Update)
                    {
                        await _feedCommandService.UpdateInFeedsAsync(postEvent);
                    }
                    else if(postEvent.EventType == EventType.Delete)
                    {
                        await _feedCommandService.RemoveFromFeedsAsync(postEvent);
                    }
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer, cancellationToken: _cancellationToken);
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
