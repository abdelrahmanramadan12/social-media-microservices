using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Events;

namespace Service.Interfaces.PostServices
{
    public interface IPostAddedService
    {
        Task HandlePostAddedAsync(PostAddedEvent post );
    }
}
