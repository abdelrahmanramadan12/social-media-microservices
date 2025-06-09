using Application.DTOs;

namespace Application.Abstractions
{
    public interface IProfileCache
    {
        Task AddProfilesAsync(Dictionary<string, ProfileDTO> profiles);
        Task<List<ProfileDTO>> GetProfilesAsync(IEnumerable<string> userIds);
    }
}
