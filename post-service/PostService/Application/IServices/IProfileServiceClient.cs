using Application.DTOs;

namespace Application.IServices
{
    public interface IProfileServiceClient
    {
        public Task<ProfilesResponse> GetProfilesAsync(ProfilesRequest profilesRequest);
    }
}
