using Application.DTOs;
using Application.DTOs.Reactions;

namespace Application.Services.Interfaces
{
    public interface IReactionServiceClient
    {
        Task<PaginationResponseWrapper<List<string>>> GetReactsOfPostAsync(GetReactsOfPostRequest request);
    }
}
