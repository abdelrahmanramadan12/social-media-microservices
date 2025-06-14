﻿using Domain.Events;
using Microsoft.Extensions.Configuration;
using Service.Interfaces.RabbitMqServices;

namespace Service.Implementations.RabbitMqServices
{
    public class ProfilePublisher : IProfilePublisher
    {
        private readonly IRabbitMqPublisher _bus;
        private readonly List<string> _queueName;

        public ProfilePublisher(IRabbitMqPublisher bus, IConfiguration config)
        {
            _bus = bus;
            _queueName = config.GetSection("RabbitQueues:ProfileQueue").GetChildren().Select(c => c.Value).ToList()!;

        }
        public Task PublishAsync(ProfileEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, _queueName, ct);

    }
}
