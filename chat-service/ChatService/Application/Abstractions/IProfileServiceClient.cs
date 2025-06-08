using Application.DTOs;

namespace Application.Abstractions
{
    public interface IProfileServiceClient
    {
        Task<ResponseWrapper<List<ProfileDTO>>> GetProfilesAsync(List<string> userIds);
    }
}
