using Domain.DTOs;
using Domain.Entities;
using Domain.IRepository;
using Service.Interfaces;

namespace Service.Implementations
{
    public class ProfileService :IProfileService
    {

        private readonly IProfileRepository _profileRepository;
        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<ProfileResponseDto?> GetByUserIdAsync(string userId)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                profile = null
            };

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.Success = false;

            }
            else
            {

                var profile = await _profileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.profile = profile;
                }
            }
            return response;

        }


        public async Task<ProfileResponseDto?> GetByUserNameAsync(string userName)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                profile = null
            };
            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "User name cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var profile = await _profileRepository.GetByUserNameAsync(userName);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.profile = profile;
                }
            }
            return response;

        }


        public async Task<MinProfileResponseDto?> GetByUserIdMinAsync(string userId)
        {
            var response = new MinProfileResponseDto
            {
                Success = true,
                Errors = null,
                SimpleUser = null
            };
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var simpleUser = await _profileRepository.GetByUserIdMinAsync(userId);
                if (simpleUser == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.SimpleUser = simpleUser;
                }
            }
            return response;
        }

        public async Task<MinProfileResponseDto?> GetByUserNameMinAsync(string userName)
        {
            var response = new MinProfileResponseDto
            {
                Success = true,
                Errors = null,
                SimpleUser = null
            };
            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "User name cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var simpleUser = await _profileRepository.GetByUserNameMinAsync(userName);
                if (simpleUser == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.SimpleUser = simpleUser;
                }
            }
            return response;
        }


        public async Task<ProfileListResponseDto?> GetUsersByIdsAsync(List<string> userIds)
        {

            var response = new ProfileListResponseDto
            {
                Success = true,
                Errors = null,
                SimpleUsers = null
            };
            if (userIds == null || userIds.Count == 0)
            {
                response.Errors = new List<string> { "User IDs cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var simpleUsers = await _profileRepository.GetUsersByIdsAsync(userIds);
                if (simpleUsers == null || simpleUsers.Count == 0)
                {
                    response.Errors = new List<string> { "No profiles found for the provided user IDs." };
                    response.Success = false;
                }
                else
                {
                    response.SimpleUsers = simpleUsers;
                }
            }
            return response;

        }

        public async Task<ProfileResponseDto?> AddAsync(Profile profile)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                profile = null
            };
            if (profile == null)
            {
                response.Errors = new List<string> { "Profile cannot be null." };
                response.Success = false;
            }
            else
            {
                await _profileRepository.AddAsync(profile);
                response.profile = profile;
            }
            return response;
        }

        public async Task<ProfileResponseDto?> UpdateAsync(string userId, Profile profile)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                profile = null
            };
            if (string.IsNullOrWhiteSpace(userId) || profile == null)
            {
                response.Errors = new List<string> { "User ID and profile cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var exists = await _profileRepository.ExistsByUserIdAsync(userId);
                if (!exists)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    profile.UserId = userId;
                    _profileRepository.Update(profile);
                    response.profile = profile;
                }
            }
            return response;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }
            await _profileRepository.DeleteAsync(userId);
            return true;
        }
    }
}
