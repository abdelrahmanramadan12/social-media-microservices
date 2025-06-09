using Application.Events;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Application.DTOs;

namespace Application.Implementations
{
    public class PostEntityQueuePublisher : IQueuePublisher<PostEvent>
    {
        private IConnection _connection;
        private IChannel _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private int _port;
        private List<string> _queueNames;

        public PostEntityQueuePublisher(IConfiguration config)
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
                HostName = _hostName,
                //Port = _port
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

            var publishTasks = _queueNames.Select(async (_queueName) =>
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
            await Task.WhenAll(publishTasks);
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
