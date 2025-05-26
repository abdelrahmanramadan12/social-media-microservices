using Application.DTOs;

namespace Application.Abstractions
{
    internal interface IProfileServiceClient
    {
        Task<ProfileDTO> GetProfileAsync(string userId);
    }
}
