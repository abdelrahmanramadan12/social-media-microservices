using Application.DTOs;

namespace Application.Abstractions
{
    public interface IFollowQueryService
    {
        Task<FollowProfilesPageDTO> ListFollowingProfilesPage(string userId, string? cursor);
        Task<FollowProfilesPageDTO> ListFollowersProfilesPage(string userId, string? cursor);
    }
}
