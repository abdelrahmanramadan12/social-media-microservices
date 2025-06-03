using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public  interface ICommentNotificationService
    {
        Task UpdatCommentListNotification(CommentEvent commentEvent);

    }
}
