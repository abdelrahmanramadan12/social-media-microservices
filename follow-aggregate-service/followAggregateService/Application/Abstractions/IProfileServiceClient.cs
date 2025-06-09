using Application.DTOs;

namespace Application.Abstractions
{
    public interface IProfileServiceClient
    {
        Task<Response<ProfilesListDTO>> GetProfilesAsync(List<string>? follows);
    }
}
