using Application.DTOs;
using Application.Events;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class PostQueuePublisher : IQueuePublisher<PostEvent>
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private int _port;
        private List<string> _queueNames;

        public PostQueuePublisher(IConfiguration config)
        {
            _userName = config.GetSection("PostMQ:UserName").Value!;
            _password = config.GetSection("PostMQ:Password").Value!;
            _hostName = config.GetSection("PostMQ:HostName").Value!;
            _queueNames = config.GetSection("PostMQ:QueueName").Value!.Split(";").ToList();
            _port = Convert.ToInt32(config.GetSection("PostMQ:Port").Value);
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                //UserName = _userName,
                //Password = _password,
                //Uri = new Uri("amqp://guest:guest@localhost:5672/")
                //Port = _port,
                HostName = _hostName
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task PublishAsync(PostEvent args)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            var message = JsonSerializer.Serialize(args);
            var bin = Encoding.UTF8.GetBytes(message);

            _queueNames.ForEach(async (_queueName) =>
            {
                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties
                    {
                        Persistent = true
                    },
                    body: bin
                );
            });
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
