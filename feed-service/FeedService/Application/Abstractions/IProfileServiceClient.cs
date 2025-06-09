using Application.DTOs;

namespace Application.Abstractions
{
    public interface IProfileServiceClient
    {
        Task<Response<ProfileDTO>> GetProfileAsync(string userId);
    }
}
