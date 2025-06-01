using Application.Abstractions;
using Application.DTOs;

namespace Application.Services
{
    public class FollowQueryService : IFollowQueryService
    {
        private readonly IFollowServiceClient _followServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;

        public FollowQueryService(
            IFollowServiceClient followServiceClient,
            IProfileServiceClient profileServiceClient)
        {
            _followServiceClient = followServiceClient;
            _profileServiceClient = profileServiceClient;
        }

        public async Task<FollowProfilesPageDTO> ListFollowingProfilesPage(string userId, string? cursor)
        {
            FollowProfilesPageDTO profiles = new();

            var followsRes = await _followServiceClient.ListFollowingPage(userId, cursor);
            if (followsRes.Success && followsRes.Value != null && followsRes?.Value?.Follows?.Count > 0)
            {
                profiles.Next = followsRes.Value.Next;

                var profilesRes = await _profileServiceClient.GetProfilesAsync(followsRes.Value.Follows);
                if (profilesRes.Success)
                {
                    profiles.Profiles = profilesRes.Value.Profiles;
                }
            }
            return profiles;
        }

        public async Task<FollowProfilesPageDTO> ListFollowersProfilesPage(string userId, string? cursor)
        {
            FollowProfilesPageDTO profiles = new();

            var followsRes = await _followServiceClient.ListFollowersPage(userId, cursor);
            if (followsRes.Success && followsRes.Value != null && followsRes?.Value?.Follows?.Count > 0)
            {
                profiles.Next = followsRes.Value.Next;

                var profilesRes = await _profileServiceClient.GetProfilesAsync(followsRes.Value.Follows);
                if (profilesRes.Success)
                {
                    profiles.Profiles = profilesRes.Value.Profiles;
                }
            }
            return profiles;
        }
    }
}
