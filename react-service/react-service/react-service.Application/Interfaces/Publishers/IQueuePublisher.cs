using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Publishers
{
    public interface IQueuePublisher<T> : IAsyncDisposable
    {
        Task InitializeAsync();
        Task PublishAsync(T args);
    }
}
