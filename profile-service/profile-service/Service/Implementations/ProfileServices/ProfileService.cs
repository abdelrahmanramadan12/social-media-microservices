using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.ProfileServices;
using Service.Interfaces.RabbitMqServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Implementations.ProfileServices
{
    public class ProfileService :IProfileService
    {

        private readonly IProfileRepository _profileRepository;
        private readonly IProfilePublisher _profilePublisher;
        public ProfileService(IProfileRepository profileRepository ,IProfilePublisher profilePublisher)
        {
            _profileRepository = profileRepository;
            _profilePublisher = profilePublisher;
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

                // Publish the profile creation event
                var profileEvent = new ProfileEvent
                {
                    EventType = ProfileEventType.ProfileAdded,
                    User= new SimpleUserDto
                    {
                        UserId = profile.UserId,
                        UserName = profile.UserName??"User",
                        DisplayName = profile.FirstName + " " + profile.LastName,
                        ProfilePictureUrl = profile.ProfileUrl
                    },

                };
                await _profilePublisher.PublishAsync(profileEvent);
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
                    await _profileRepository.Update(profile);
                    response.profile = profile;

                    // Publish the profile Updated event
                    var profileEvent = new ProfileEvent
                    {
                        EventType = ProfileEventType.ProfileUpdated,
                        User = new SimpleUserDto
                        {
                            UserId = profile.UserId,
                            UserName = profile.UserName,
                            DisplayName = profile.FirstName + " " + profile.LastName,
                            ProfilePictureUrl = profile.ProfileUrl
                        },

                    };
                    await _profilePublisher.PublishAsync(profileEvent);

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

            // Publish the profile Deleted event
            var profileEvent = new ProfileEvent
            {
                EventType = ProfileEventType.ProfileDeleted,
                User = new SimpleUserDto
                {
                    UserId = userId
                },

            };
            await _profilePublisher.PublishAsync(profileEvent);
            return true;
        }
    }
}
