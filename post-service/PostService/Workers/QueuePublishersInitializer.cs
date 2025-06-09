using Application.DTOs;
using Application.Events;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workers
{
    public class QueuePublishersInitializer : IHostedService
    {
        private readonly IQueuePublisher<PostEvent> _postPublisher;

        public QueuePublishersInitializer(IQueuePublisher<PostEvent> postPublisher)
        {
            _postPublisher = postPublisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postPublisher.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postPublisher.DisposeAsync();
        }
    }
}
