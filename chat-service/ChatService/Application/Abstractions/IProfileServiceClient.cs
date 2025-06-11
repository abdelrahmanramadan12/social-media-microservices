using Application.DTOs;

namespace Application.Abstractions
{
    public interface IProfileServiceClient
    {
        Task<Response<List<ProfileDTO>>> GetProfilesAsync(List<string> userIds);
    }
}
