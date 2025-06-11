using Application.DTOs;
using Application.DTOs.Responses;

namespace Application.Abstractions
{
    public interface IProfileServiceClient
    {
        Task<ResponseWrapper<ProfileDTO>> GetProfileAsync(string userId);
    }
}
