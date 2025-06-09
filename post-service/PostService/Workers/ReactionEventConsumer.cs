using Application.Events;
using Domain.Enums;
using Domain.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Workers
{
    public class ReactionEventConsumer : IHostedService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IConfiguration _configuration;
        private readonly IPostRepository _postRepository;
        private readonly string _queueName;

        public ReactionEventConsumer(IConfiguration configuration, IPostRepository postRepository)
        {
            _configuration = configuration;
            _postRepository = postRepository;
            _queueName = _configuration.GetSection("ReactionMQ:QueueName").Value!;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetSection("ReactionMQ:HostName").Value!,
                UserName = _configuration.GetSection("ReactionMQ:UserName").Value!,
                Password = _configuration.GetSection("ReactionMQ:Password").Value!,
                Port = Convert.ToInt32(_configuration.GetSection("ReactionMQ:Port").Value)
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

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
                var reactionEvent = JsonSerializer.Deserialize<ReactionEvent>(message);

                if (reactionEvent != null)
                {
                    var counterEvent = reactionEvent.ReactionType == ReactionEventType.Like
                        ? PostCounterEvent.ReactionCreated
                        : PostCounterEvent.ReactionDeleted;

                    await _postRepository.UpdatePostCountersAsync(reactionEvent.PostId, counterEvent);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}