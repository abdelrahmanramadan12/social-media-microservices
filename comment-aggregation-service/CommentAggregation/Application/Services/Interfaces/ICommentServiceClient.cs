using Application.DTOs.Application.DTOs;
using Application.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ICommentServiceClient
    {
        Task<PaginationResponseWrapper<List<CommentResponse>>> GetPagedCommentList(GetPagedCommentRequest request);
    }
}
