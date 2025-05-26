using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using reat_service.Application.Interfaces.Publishers;
namespace reat_service.Application.Services
{



    public class ReactionPublisher : IReactionPublisher
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _queueName;

        public ReactionPublisher(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQ:HostName"];
            _username = configuration["RabbitMQ:UserName"];
            _password = configuration["RabbitMQ:Password"];
            _queueName = configuration["RabbitMQ:QueueName"];
        }

        public void Publish<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Serialize object to JSON string
            var jsonString = JsonSerializer.Serialize(message);

            // Convert JSON string to byte array
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }

}
