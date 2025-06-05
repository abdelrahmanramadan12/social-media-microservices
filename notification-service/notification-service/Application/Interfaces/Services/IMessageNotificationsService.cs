using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
  
        interface IMessageNotificationService
        {
            public Task UpdatMessageListNotification(MessageEvent messageEvent);
        }
   
}
