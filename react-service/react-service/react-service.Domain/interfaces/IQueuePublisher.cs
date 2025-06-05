using react_service.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Domain.interfaces
{
    public  interface IQueuePublisher<T> : IAsyncDisposable where T : QueueEvent
    {
        Task InitializeAsync();
        Task PublishAsync(T args);
    }
}
