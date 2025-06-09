using Application.DTOs;
using Application.DTOs.Reactions;

namespace Application.Services.Interfaces
{
    public interface IReactionServiceClient
    {
        Task<PaginationResponseWrapper<string>> GetReactsOfPostAsync(GetReactsOfPostRequest request);
    }
}
