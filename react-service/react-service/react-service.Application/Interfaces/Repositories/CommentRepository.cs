using react_service.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<bool> IsCommentDeleted(string commentId);
        Task<bool> DeleteComment(string commentId);
        Task<bool> AddComment(Comment comment);
        Task<Comment> GetCommentAsync(string commentId);
    }
}
